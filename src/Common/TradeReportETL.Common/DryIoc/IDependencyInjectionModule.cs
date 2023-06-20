using DryIoc;
using Microsoft.Extensions.Configuration;

namespace TradeReportETL.Common.DryIoc
{
    public interface IDependencyInjectionModule
    {
        void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration);
    }
}