using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CartService.Session
{
    public class DdbXmlRepository : IXmlRepository
    {
        private static IAmazonDynamoDB _dynamoDb;

        public DdbXmlRepository(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {

            var context = new DynamoDBContext(_dynamoDb);
            var search = context.ScanAsync<XmlKey>(new List<ScanCondition>());
            var results = search.GetRemainingAsync().Result;

            return results.Select(x => XElement.Parse(x.Xml)).ToList();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var key = new XmlKey
            {
                Xml = element.ToString(SaveOptions.DisableFormatting),
                FriendlyName = friendlyName
            };

            var context = new DynamoDBContext(_dynamoDb);
            context.SaveAsync(key).Wait();
        }
    }

    [DynamoDBTable("AspXmlKeys")]
    public class XmlKey
    {
        [DynamoDBHashKey]
        public string KeyId { get; set; } = Guid.NewGuid().ToString();
        public string Xml { get; set; }
        public string FriendlyName { get; set; }
    }
}
