using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common.Cache;
using TradeReportETL.Shared.Messages;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Load.Services
{
    public class LoadTransactionHandler : IHandleMessages<LoadTransaction>
    {
        private readonly ILogger<LoadTransactionHandler> _logger;
        private readonly ITransactionLoadService _transactionLoadService;
        private readonly IDistributedCache _distributedCache;

        public LoadTransactionHandler(
            ILogger<LoadTransactionHandler> logger,
            IDistributedCache distributedCache,
            ITransactionLoadService transactionLoadService)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _transactionLoadService = transactionLoadService;
        }

        public async Task Handle(LoadTransaction message, IMessageHandlerContext context)
        {
            _logger.LogInformation(
                "Received TrasformTransaction Command for TransactionId {TransactionId}. Retrieving Transaction from DB...",
                message?.TransactionId);

            var transaction = await _distributedCache.GetAsync<TradeReportTransactionModel>(
                TradeReportTransactionModel.GetCacheId(message?.TransactionId), context.CancellationToken);

            _logger.LogTrace("Starting to Load Transaction {TransactionId}...", transaction.Id);

            await _transactionLoadService.LoadTransaction(transaction, context.CancellationToken);

        }

    }
}