using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TradeReportETL.Pipeline.Modules.Transform.Models;

namespace TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient
{
    public class GleifApiClient : IGleifApiClient
    {

        private readonly HttpClient _httpClient;
        public GleifApiClient(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
        }


        public async Task<ICollection<LeiModel>> GetLeiRecordsBatchAsync(ICollection<string> leiList)
        {
            var requestUri = "lei-records";
            var leiListJoined = string.Join(',', leiList);

            Dictionary<string, string> queryParams = new()
            {

                { "filter[lei]", leiListJoined },
                { "page[size]", leiList.Count.ToString() },
                { "page[number]", "1" },
            };


            requestUri = QueryHelpers.AddQueryString(requestUri, queryParams);

            using var requestMessage = CreateRequest(requestUri, HttpMethod.Get);


            using var response = await _httpClient.SendAsync(requestMessage);

            var resultString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var gleifApiResponse = JsonConvert.DeserializeObject<GleifApiResponse>(resultString)!;
                return gleifApiResponse.Data;
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
