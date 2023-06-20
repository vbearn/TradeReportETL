using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Transform.Services
{
    public interface ITransactionTransformService
    {
        Task<List<TradeReportTransactionModel>> TransformBatchTransactions(List<TradeReportTransactionModel> transaction, CancellationToken cancellationToken);
    }
}