using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Shared.Messages;

namespace TradeReportETL.Shared.Processors
{
    public interface IFileProcessorService
    {
        Task<bool> ProcessFile(IExtractCommand processEvent, CancellationToken cancellationToken);
    }
}