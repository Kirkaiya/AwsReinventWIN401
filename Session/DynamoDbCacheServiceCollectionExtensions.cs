using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.DynamoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamoDbCacheServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Amazon DynamoDB caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{DynamoDbCacheOptions}"/> to configure the provided
        /// <see cref="DynamoDbCacheOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDistributedDynamoDbCache(this IServiceCollection services, Action<DynamoDbCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<IDistributedCache, DynamoDbCache>());

            return services;
        }
    }
}
