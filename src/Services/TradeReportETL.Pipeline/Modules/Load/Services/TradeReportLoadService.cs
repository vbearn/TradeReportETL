using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common.Cache;
using TradeReportETL.Common.Http;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Load.Services
{
    public class TradeReportLoadService : ITradeReportLoadService
    {
        private readonly ILogger<TradeReportLoadService> _logger;
        private readonly IDistributedCache _distributedCache;
        public TradeReportLoadService(ILogger<TradeReportLoadService> logger,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<float> GetTradeReportProgressPercentage(string tradeReportId, CancellationToken cancellationToken)
        {
            var tradeReport = await _distributedCache.GetAsync<TradeReportModel>(
                      TradeReportModel.GetCacheId(tradeReportId), cancellationToken);

            var progressPercentage = (float)tradeReport.FinishedTransactionsCount / (float)tradeReport.TotalTransactionsCount * 100;

            return progressPercentage;
        }

        public async Task GetTradeReportTransactions(string tradeReportId, StreamWriter streamWriter, CancellationToken cancellationToken)
        {
            var tradeReport = await _distributedCache.GetAsync<TradeReportModel>(
                      TradeReportModel.GetCacheId(tradeReportId), cancellationToken);

            if (tradeReport.FinishedTransactionsCount < tradeReport.TotalTransactionsCount)
            {
                _logger.LogError("Trade Report Process Not Finished {tradeReportId}...", tradeReportId);
                throw new NotFoundException("Trade Report Process Not Finished");
            }


            _logger.LogInformation("Start writing Json to output stream {tradeReportId}...", tradeReportId);
            var jsonTextWriter = new JsonTextWriter(streamWriter)
            {
                Formatting = Formatting.Indented
            };

            await jsonTextWriter.WriteStartArrayAsync(cancellationToken);


            foreach (var transactionId in tradeReport.TransactionIds)
            {
                var transaction = await _distributedCache.GetAsync<TradeReportTransactionModel>(
                     TradeReportTransactionModel.GetCacheId(transactionId), cancellationToken);

                await WriteTransactionToJson(jsonTextWriter, transaction, cancellationToken);

                await streamWriter.FlushAsync().ConfigureAwait(false);
            }

            await jsonTextWriter.WriteEndArrayAsync(cancellationToken);

            _logger.LogInformation("Finished writing Json to output stream {tradeReportId}.", tradeReportId);
        }

        private static async Task WriteTransactionToJson(JsonTextWriter jsonTextWriter, TradeReportTransactionModel transaction, CancellationToken cancellationToken)
        {
            await jsonTextWriter.WriteStartObjectAsync(cancellationToken);
            {
                await jsonTextWriter.WritePropertyNameAsync("transaction_uti", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.TransactionUti, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("isin", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.Isin, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("notional", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.Notional, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("notional_currency", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.NotionalCurrency, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("transaction_type", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.TransactionType, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("transaction_datetime", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.TransactionDateTime, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("rate", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.Rate, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("lei", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.Lei, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("legalName", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.LegalName, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("bic", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.Bic, cancellationToken);

                await jsonTextWriter.WritePropertyNameAsync("transactions_costs", cancellationToken);
                await jsonTextWriter.WriteValueAsync(transaction.TransactionCost, cancellationToken);
            }
            await jsonTextWriter.WriteEndObjectAsync(cancellationToken);
        }
    }
}