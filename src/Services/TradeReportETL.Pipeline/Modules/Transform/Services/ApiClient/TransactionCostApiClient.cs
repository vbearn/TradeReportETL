using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient
{
    public class TransactionCostApiClient : ITransactionCostApiClient
    {

        private readonly HttpClient _httpClient;
        public TransactionCostApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<decimal> CalculateTransactionCost(string country, decimal rate, decimal notional)
        {
            var requestUri = "TransactionCost";

            Dictionary<string, string> queryParams = new()
            {
                { "country", country },
                { "rate", rate.ToString() },
                { "notional", notional.ToString() },
            };


            requestUri = QueryHelpers.AddQueryString(requestUri, queryParams);

            using var requestMessage = CreateRequest(requestUri, HttpMethod.Get);


            using var response = await _httpClient.SendAsync(requestMessage);

            var resultString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var gleifApiResponse = JsonConvert.DeserializeObject<decimal>(resultString)!;
                return gleifApiResponse;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }

            throw new Exception($"GleifApi responded with error: {resultString}");
        }

        private static HttpRequestMessage CreateRequest(string uri, HttpMethod httpMethod, StringContent requestBody = null)
        {
            return new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(uri, UriKind.Relative),
                Content = requestBody
            };
        }

    }
}
