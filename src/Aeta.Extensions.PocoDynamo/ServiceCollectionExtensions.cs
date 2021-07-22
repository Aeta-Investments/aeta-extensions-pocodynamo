using System;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Aws.DynamoDb;

namespace Aeta.Extensions.PocoDynamo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmazonDynamoDbClient(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped,
            Action<AmazonDynamoDBConfig> configure = null)
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(IAmazonDynamoDB), serviceProvider =>
            {
                var config = serviceProvider.GetAmazonDynamoDbConfig();
                configure?.Invoke(config);

                return new AmazonDynamoDBClient(config);
            }, lifetime);

            services.Add(serviceDescriptor);
            return services;
        }

        public static IServiceCollection AddPocoDynamo(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped,
            Action<IPocoDynamo> configure = null)
        {
            object PocoDynamoFactory(IServiceProvider serviceProvider)
            {
                var pocoDynamo =
                    new ServiceStack.Aws.DynamoDb.PocoDynamo(serviceProvider.GetRequiredService<IAmazonDynamoDB>());
                configure?.Invoke(pocoDynamo);

                return pocoDynamo;
            }

            services.AddAmazonDynamoDbClient();
            services.Add(new ServiceDescriptor(typeof(IPocoDynamo), PocoDynamoFactory, lifetime));
            services.Add(new ServiceDescriptor(typeof(IPocoDynamoAsync), PocoDynamoFactory, lifetime));

            return services;
        }

        private static AmazonDynamoDBConfig GetAmazonDynamoDbConfig(this IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()?
                .GetSection("Amazon")?
                .GetSection("Dynamo");

            var amazonConfig = new AmazonDynamoDBConfig();
            if (configuration is null) return amazonConfig;

            configuration.Bind(amazonConfig);
            return amazonConfig;
        }
    }
}