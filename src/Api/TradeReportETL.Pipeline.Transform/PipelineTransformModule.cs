using DryIoc;
using Microsoft.Extensions.Configuration;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.Pipeline.Modules.Transform.Services;
using TradeReportETL.Shared.Services;

namespace TradeReportETL.Pipeline.Transform
{
    public class PipelineTransformModule : IDependencyInjectionModule
    {
        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<ITransactionTransformService, TransactionTransformService>(Reuse.Scoped);

            service.Register<RedisLockService>(Reuse.Singleton);
        }
    }
}