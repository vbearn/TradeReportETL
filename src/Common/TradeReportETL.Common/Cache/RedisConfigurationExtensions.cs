using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace TradeReportETL.Common.Cache
{
    public static class RedisConfigurationExtensions
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints =
                    {
                        {
                            configuration.GetValue<string>("Redis:Host"),
                            configuration.GetValue<int>("Redis:Port")
                        }
                    },
                AbortOnConnectFail = false
            });

            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = redis.Configuration;
                o.InstanceName = "TradeReportETLCache";
            });
            return services;
        }
    }
}