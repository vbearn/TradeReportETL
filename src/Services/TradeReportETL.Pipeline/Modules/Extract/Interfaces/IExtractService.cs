using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Shared.Messages;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Extract.Interfaces
{
    public interface IExtractService
    {
        Task<IAsyncEnumerable<TradeReportTransactionModel>> ExtractFile(IExtractCommand processEvent,
            CancellationToken cancellationToken);
    }
}