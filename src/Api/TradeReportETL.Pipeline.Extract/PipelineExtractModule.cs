using DryIoc;
using Microsoft.Extensions.Configuration;
using TradeReportETL.Pipeline.Modules.Extract.Services.Csv;
using TradeReportETL.Pipeline.Modules.Extract.Interfaces;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.Shared.Services;

namespace TradeReportETL.Pipeline.Extract
{
    public class PipelineExtractModule : IDependencyInjectionModule
    {
        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<IExtractService, CsvTradeReportExtractService>(Reuse.Scoped);

            service.Register<RedisLockService>(Reuse.Singleton);
        }
    }
}