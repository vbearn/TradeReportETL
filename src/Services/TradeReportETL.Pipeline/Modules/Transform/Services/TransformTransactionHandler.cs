using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common.Cache;
using TradeReportETL.Shared.Messages;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Transform.Services
{
    public class TransformTransactionHandler : IHandleMessages<TransformTransactions>
    {
        private readonly ILogger<TransformTransactionHandler> _logger;
        private readonly ITransactionTransformService _transactionTransformService;
        private readonly IDistributedCache _distributedCache;

        public TransformTransactionHandler(
            ILogger<TransformTransactionHandler> logger,
            IDistributedCache distributedCache,
            ITransactionTransformService transactionTransformService)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _transactionTransformService = transactionTransformService;
        }

        public async Task Handle(TransformTransactions message, IMessageHandlerContext context)
        {
            _logger.LogInformation(
                "Received TrasformTransactions Command for first TransactionId {TransactionId}. Retrieving Transaction from DB...",
                message?.TransactionIds.FirstOrDefault());

            var transactions = new List<TradeReportTransactionModel>();
            foreach (var transactionId in message?.TransactionIds)
            {
                _logger.LogTrace("Fetching Transaction {TransactionId} from DB for Trasform ...", transactionId);

                var transaction = await _distributedCache.GetAsync<TradeReportTransactionModel>(
                    TradeReportTransactionModel.GetCacheId(transactionId), context.CancellationToken);

                transactions.Add(transaction);
            }


            transactions = await _transactionTransformService.TransformBatchTransactions(transactions, context.CancellationToken);

            foreach (var transaction in transactions)
            {
                _logger.LogTrace("Saving trasformed Transaction{TransactionId} to DB...", transaction.Id);

                await _distributedCache.SetAsync(TradeReportTransactionModel.GetCacheId(transaction.Id), transaction,
                    new DistributedCacheEntryOptions { }, context.CancellationToken);

                await context.Send(new LoadTransaction() { TransactionId = transaction.Id });
            }
           

            _logger.LogInformation("Finalized trasforming Transactions for first TransactionId {TransactionId} ...",
                message?.TransactionIds.FirstOrDefault());

        }

    }
}