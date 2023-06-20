using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
//using Serilog.Context;

namespace TradeReportETL.Common.Http
{
    public sealed class LogRequestResponseHandler : DelegatingHandler
    {
        private const string NonJsonLogMessage = "{Non-json request/response}";
        private readonly ILogger _logger;

        public LogRequestResponseHandler(ILogger<LogRequestResponseHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestMethod = request.Method.Method;
            var requestBody = string.Empty;
            var responseBody = string.Empty;

            var requestTime = new Stopwatch();
            try
            {
                if (request.Content != null)
                {
                    requestBody = await request.Content.ReadAsStringAsync();
                }

                _logger.LogInformation("Request uri: {ExternalRequestUri}", request.RequestUri);

                requestTime.Start();
                var response = await base.SendAsync(request, cancellationToken);
                requestTime.Stop();

                if (response.Content != null)
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                }

                var logLevel = LogLevel.Information;
                if ((int)response.StatusCode >= 500)
                {
                    logLevel = LogLevel.Error;
                }
                else if ((int)response.StatusCode >= 400)
                {
                    logLevel = LogLevel.Warning;
                }

                //using (LogContext.PushProperty("ExternalRequestBody", requestBody))
                //using (LogContext.PushProperty("ExternalResponseBody", responseBody))
                _logger.Log(logLevel,
                    "Request uri: {ExternalRequestUri}. Status code: {ExternalRequestStatusCode}, Request method: {ExternalRequestMethod},Request Time: {ExternalRequestTime}ms", request.RequestUri,
                    response.StatusCode, requestMethod, requestTime.ElapsedMilliseconds);

                return response;
            }
            catch (Exception exception)
            {
                requestTime.Stop();
                //using (LogContext.PushProperty("ExternalRequestBody", requestBody))
                //using (LogContext.PushProperty("ExternalResponseBody", responseBody))
                _logger.LogError(
                    exception,
                    "Exception while sending request/receiving response. Request uri: {ExternalRequestUri}, Request method {ExternalRequestMethod}, Request Time: {ExternalRequestTime}ms",
                    request?.RequestUri, requestMethod, requestTime.ElapsedMilliseconds);

                throw;
            }
        }
    }
}