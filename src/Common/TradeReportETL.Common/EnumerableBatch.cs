using System;
using System.Collections.Generic;

namespace TradeReportETL.Common
{
    public static class EnumerableBatch
    {
        public static async IAsyncEnumerable<T[]> Batch<T>(
        this IAsyncEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            await foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                Array.Resize(ref bucket, count);
                yield return bucket;
            }
        }
    }
}
