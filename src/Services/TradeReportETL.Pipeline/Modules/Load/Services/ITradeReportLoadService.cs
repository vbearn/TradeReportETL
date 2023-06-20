using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TradeReportETL.Pipeline.Modules.Load.Services
{
    public interface ITradeReportLoadService
    {
        Task<float> GetTradeReportProgressPercentage(string tradeReportId, CancellationToken cancellationToken);
        Task GetTradeReportTransactions(string tradeReportId, StreamWriter streamWriter, CancellationToken cancellationToken);
    }
}