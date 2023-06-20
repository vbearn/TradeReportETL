using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common.Cache;
using TradeReportETL.Shared.Models;
using TradeReportETL.Shared.Services;

namespace TradeReportETL.Pipeline.Modules.Load.Services
{
    public class TransactionLoadService : ITransactionLoadService
    {
        private readonly ILogger<TransactionLoadService> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly RedisLockService _distributedLock;
        public TransactionLoadService(ILogger<TransactionLoadService> logger,
            IDistributedCache distributedCache, RedisLockService distributedLock)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _distributedLock = distributedLock;
        }

        public async Task LoadTransaction(TradeReportTransactionModel transaction,
            CancellationToken cancellationToken)
        {
            // Perform any necessary Transaction Load, e.g: Writing to a CSV/DataLake/... 

            // Increase FinishedTransactionsCount to track import progress

            long totalTransactionsCount = 0;
            long finishedTransactionsCount = 0;

            var lockSuccess = false;
            while (!lockSuccess)
            {
                await using (var redLock = await _distributedLock.RedlockFactory.CreateLockAsync(
                     TradeReportModel.GetCacheId(transaction.TradeReportId), TimeSpan.FromSeconds(5)))
                {
                    // make sure we got the lock
                    if (redLock.IsAcquired)
                    {
                        var tradeReport = await _distributedCache.GetAsync<TradeReportModel>(
                            TradeReportModel.GetCacheId(transaction.TradeReportId), cancellationToken);

                        tradeReport.FinishedTransactionsCount = tradeReport.FinishedTransactionsCount + 1;
                        totalTransactionsCount = tradeReport.TotalTransactionsCount;
                        finishedTransactionsCount = tradeReport.FinishedTransactionsCount;

                        await _distributedCache.SetAsync(TradeReportModel.GetCacheId(tradeReport.Id), tradeReport,
                            new DistributedCacheEntryOptions { }, cancellationToken);

                        lockSuccess = true;
                    }
                }
            }


            _logger.LogTrace("Finished Loading Transaction {finishedTransactionsCount} of {totalTransactionsCount}...", finishedTransactionsCount, totalTransactionsCount);

        }

    }
}