using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TradeReportETL.Common.Http;

namespace TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient
{
    public static class ApiClientsServiceCollectionExtension
    {
        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddTransient<LogRequestResponseHandler>();

            services.AddHttpClient<IGleifApiClient, GleifApiClient>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(configuration.GetValue<string>("GleifApi:BaseUrl"));
            })
            .AddRetriesForTransientErrorsOnGet()
            .AddHttpMessageHandler<LogRequestResponseHandler>();


            services.AddHttpClient<ITransactionCostApiClient, TransactionCostApiClient>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(configuration.GetValue<string>("TransactionCostApi:BaseUrl"));
            })
            .AddRetriesForTransientErrorsOnGet()
            .AddHttpMessageHandler<LogRequestResponseHandler>();

            return services;
        }
    }
}
