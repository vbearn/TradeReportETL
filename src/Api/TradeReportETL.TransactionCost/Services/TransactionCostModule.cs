using DryIoc;
using Microsoft.Extensions.Configuration;
using TradeReportETL.Common.DryIoc;
using TradeReportETL.TransactionCost.Services.TransactionCost;

namespace TradeReportETL.TransactionCost.Services
{
    public class TransactionCostModule : IDependencyInjectionModule
    {
        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<ITransactionCostCalculatorService, TransactionCostCalculatorService>(Reuse.Scoped);

        }
    }
}