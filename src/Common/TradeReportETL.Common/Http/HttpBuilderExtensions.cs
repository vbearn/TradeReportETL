using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace TradeReportETL.Common.Http
{
    public static class HttpBuilderExtensions
    {
        private static readonly IAsyncPolicy<HttpResponseMessage> RetryPolicyForGet = HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(3, retry => retry * TimeSpan.FromMinutes(1));

        private static readonly IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicyForGet = HttpPolicyExtensions.HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        private static readonly IAsyncPolicy<HttpResponseMessage> NoOpPolicy = Policy.NoOpAsync<HttpResponseMessage>();

        /// <summary>
        /// Retries Get request on transient error 3 times linearly increasing waiting time 1sec/attempt.
        /// </summary>
        public static IHttpClientBuilder AddRetriesForTransientErrorsOnGet(this IHttpClientBuilder builder) =>
            builder.ProvidedNotNull(nameof(builder))
                .AddPolicyHandler(r => r.Method == HttpMethod.Get ? RetryPolicyForGet : NoOpPolicy)
                .AddPolicyHandler(r => r.Method == HttpMethod.Get ? CircuitBreakerPolicyForGet : NoOpPolicy);

      
    }
}