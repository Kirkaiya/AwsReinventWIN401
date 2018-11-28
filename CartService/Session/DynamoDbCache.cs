using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Caching.DynamoDb
{
    public class DynamoDbCache : IDistributedCache
    {
        private static IAmazonDynamoDB _client;
        private static Table _table;

        private string _tableName = "ASP.NET_SessionState";
        private string _ttlfield = "TTL";
        private int _sessionMinutes = 20;
        private enum ExpiryType
        {
            Sliding,
            Absolute
        }

        public DynamoDbCache(IOptions<DynamoDbCacheOptions> optionsAccessor, IAmazonDynamoDB dynamoDb)
        {
            _client = dynamoDb;

            if (optionsAccessor != null)
            {
                _tableName = optionsAccessor.Value.TableName;
                _ttlfield = optionsAccessor.Value.TtlAttribute;
                _sessionMinutes = (int)optionsAccessor.Value.IdleTimeout.TotalMinutes;
            }

            if (_client == null)
            {
                _client = new AmazonDynamoDBClient();
            }

            if (_table == null)
            {
                _table = Table.LoadTable(_client, _tableName);
            }
        }

        public byte[] Get(string key)
        {
            return GetAsync(key).Result;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var value = await _table.GetItemAsync(key);
            if (value == null || value["Session"] == null)
            {
                return null;
            }

            return value["Session"].AsByteArray();
        }

        public void Refresh(string key)
        {
            var value = _table.GetItemAsync(key).Result;
            if (value == null || value["ExpiryType"] == null || value["ExpiryType"] != "Sliding")
            {
                return;
            }
            value[_ttlfield] = DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds() + (_sessionMinutes * 60);

            Task.Run(() => Set(key, value["Session"].AsByteArray(), new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0, _sessionMinutes, 0) }));
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var value = _table.GetItemAsync(key).Result;
            if (value == null || value["ExpiryType"] == null || value["ExpiryType"] != "Sliding")
            {
                return;
            }
            value[_ttlfield] = DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds() + (_sessionMinutes * 60);

            await SetAsync(key, value["Session"].AsByteArray(), new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0, _sessionMinutes, 0) });
        }

        public void Remove(string key)
        {
            _table.DeleteItemAsync(key).Wait();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await _table.DeleteItemAsync(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).Wait();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            ExpiryType expiryType;
            var epoctime = GetEpochExpiry(options, out expiryType);
            var _ssdoc = new Document();

            _ssdoc.Add("SessionId", key);
            _ssdoc.Add("Session", value);
            _ssdoc.Add("CreateDate", DateTime.Now.ToUniversalTime().ToString("o"));
            _ssdoc.Add("ExpiryType", expiryType.ToString());
            _ssdoc.Add(_ttlfield, epoctime);

            await _table.PutItemAsync(_ssdoc);
        }

        private long GetEpochExpiry(DistributedCacheEntryOptions options, out ExpiryType expiryType)
        {
            if (options.SlidingExpiration.HasValue)
            {
                expiryType = ExpiryType.Sliding;
                return DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds() + (long)options.SlidingExpiration.Value.TotalSeconds;

            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                expiryType = ExpiryType.Absolute;
                return options.AbsoluteExpiration.Value.ToUnixTimeSeconds();

            }
            else if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                expiryType = ExpiryType.Absolute;
                return DateTimeOffset.Now.Add(options.AbsoluteExpirationRelativeToNow.Value).ToUniversalTime().ToUnixTimeSeconds();
            }
            else
            {
                throw new Exception("Cache expiry option must be set to Sliding, Absolute or Absolute relative to now");
            }
        }
    }
}
