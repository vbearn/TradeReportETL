using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Transform.Services
{
    public class TransactionTransformService : ITransactionTransformService
    {
        private readonly IGleifApiClient _gleifApiClient;
        private readonly ITransactionCostApiClient _transactionCostApiClient;
        
        private readonly ILogger<TransactionTransformService> _logger;


        public TransactionTransformService(
            IGleifApiClient gleifApiClient, ILogger<TransactionTransformService> logger, ITransactionCostApiClient transactionCostApiClient)
        {

            _gleifApiClient = gleifApiClient;
            _logger = logger;
            _transactionCostApiClient = transactionCostApiClient;
        }

        public async Task<List<TradeReportTransactionModel>> TransformBatchTransactions(
            List<TradeReportTransactionModel> transactions,
            CancellationToken cancellationToken)
        {
          
            _logger.LogInformation("Start transforming transaction batch with first Id {transactionId} ...", transactions.FirstOrDefault()?.Id);

            var leiList = await _gleifApiClient.GetLeiRecordsBatchAsync(transactions.Select(t => t.Lei).ToArray());

            foreach (var leiRecord in leiList)
            {
                var transaction = transactions.FirstOrDefault(t => t.Lei == leiRecord.Attributes.Lei);
                transaction.LegalName = leiRecord.Attributes.Entity.LegalName.Name;
                transaction.Bic = leiRecord.Attributes.Bic.FirstOrDefault();
                transaction.Country = leiRecord.Attributes.Entity.LegalAddress.Country;

                transaction.TransactionCost = await _transactionCostApiClient.CalculateTransactionCost(
                    transaction.Country, transaction.Rate, transaction.Notional);

            }

            return transactions;
        }
    }
}