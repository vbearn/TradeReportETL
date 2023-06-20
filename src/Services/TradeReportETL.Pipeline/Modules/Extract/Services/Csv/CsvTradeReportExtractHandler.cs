using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common;
using TradeReportETL.Common.Cache;
using TradeReportETL.Pipeline.Modules.Extract.Interfaces;
using TradeReportETL.Shared.Messages;
using TradeReportETL.Shared.Models;
using TradeReportETL.Shared.Services;

namespace TradeReportETL.Pipeline.Modules.Extract.Services.Csv
{
    public class CsvTradeReportExtractHandler : IHandleMessages<ExtractCsvFile>
    {
        private readonly ILogger<CsvTradeReportExtractHandler> _logger;
        private readonly IExtractService _extractService;
        private readonly RedisLockService _distributedLock;
        private readonly IDistributedCache _distributedCache;

        public const int BatchSize = 40;
        public CsvTradeReportExtractHandler(
            ILogger<CsvTradeReportExtractHandler> logger,
            IExtractService extractService,
            IDistributedCache distributedCache,
            RedisLockService distributedLock)
        {
            _logger = logger;
            _extractService = extractService;
            _distributedCache = distributedCache;
            _distributedLock = distributedLock;
        }

        public async Task Handle(ExtractCsvFile message, IMessageHandlerContext context)
        {
            _logger.LogInformation(
                "Received ProcessCsvFile Command for file {PathOnDisk}. Starting to write trade report to DB...",
                message?.FileName);

            var tradeReport = new TradeReportModel() { Id = message.Id };

            await _distributedCache.SetAsync(TradeReportModel.GetCacheId(tradeReport.Id),
                tradeReport, new DistributedCacheEntryOptions { }, context.CancellationToken);

            _logger.LogInformation(
              "Starting to extract file {PathOnDisk}... TradeReport Id is {tradeReportId}",
              message?.FileName, tradeReport.Id);

            var transactions = await _extractService.ExtractFile(message, context.CancellationToken);

            var transactionIdList = new List<string>();

            await foreach (TradeReportTransactionModel[] transactionBatch in transactions.Batch(BatchSize))
            {

                foreach (var transaction in transactionBatch)
                {

                    transaction.Id = Guid.NewGuid().ToString();
                    transaction.TradeReportId = tradeReport.Id;

                    _logger.LogTrace("Adding transaction {transactionId} from TradeReport {tradeReportId} to cache...", transaction.Id, tradeReport.Id);

                    await _distributedCache.SetAsync(TradeReportTransactionModel.GetCacheId(transaction.Id),
                        transaction, new DistributedCacheEntryOptions { }, context.CancellationToken);

                    transactionIdList.Add(transaction.Id);

                }

                await context.Send(new TransformTransactions()
                {
                    TransactionIds = transactionBatch.Select(t => t.Id).ToArray()
                });

            }

            // update the tradeReport while acquiring a lock, to make sure Transform pipeline wouldn't interfere with the value mid-update
            var lockSuccess = false;
            while (!lockSuccess)
            {
                await using (var redLock = await _distributedLock.RedlockFactory.CreateLockAsync(
                TradeReportModel.GetCacheId(tradeReport.Id), TimeSpan.FromMinutes(1)))
                {
                    if (redLock.IsAcquired)
                    {
                        tradeReport = await _distributedCache.GetAsync<TradeReportModel>(
                            TradeReportModel.GetCacheId(tradeReport.Id), context.CancellationToken);

                        tradeReport.TransactionIds = transactionIdList.ToArray();
                        tradeReport.TotalTransactionsCount = transactionIdList.Count;

                        await _distributedCache.SetAsync(TradeReportModel.GetCacheId(tradeReport.Id), tradeReport,
                            new DistributedCacheEntryOptions { }, context.CancellationToken);

                        lockSuccess = true;
                    }
                }
            }


            _logger.LogInformation("Finalized extracting file {FileName} ...", message?.FileName);

        }

    }
}