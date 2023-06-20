using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace TradeReportETL.Common.Cache
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value,
            DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            await distributedCache.SetAsync(key, value.ToByteArray(), options, token);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key,
            CancellationToken token = default) where T : class
        {
            var result = await distributedCache.GetAsync(key, token);
            return result.FromByteArray<T>();
        }

        private static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        private static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(byteArray));
        }
    }
}