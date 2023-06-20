using DryIoc;
using Microsoft.Extensions.Configuration;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.Pipeline.Modules.Load.Services;
using TradeReportETL.Shared.Services;

namespace TradeReportETL.Pipeline.Load
{
    public class PipelineLoadModule : IDependencyInjectionModule
    {
        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<ITransactionLoadService, TransactionLoadService>(Reuse.Scoped);

            service.Register<RedisLockService>(Reuse.Singleton);

        }
    }
}