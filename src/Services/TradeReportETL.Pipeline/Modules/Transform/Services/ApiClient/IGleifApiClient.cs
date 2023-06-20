using System.Collections.Generic;
using System.Threading.Tasks;
using TradeReportETL.Pipeline.Modules.Transform.Models;

namespace TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient
{
    public interface IGleifApiClient
    {
        Task<ICollection<LeiModel>> GetLeiRecordsBatchAsync(ICollection<string> leiList);
    }
}