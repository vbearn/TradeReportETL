using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Load.Services
{
    public interface ITransactionLoadService
    {
        Task LoadTransaction(TradeReportTransactionModel transaction, CancellationToken cancellationToken);
    }
}