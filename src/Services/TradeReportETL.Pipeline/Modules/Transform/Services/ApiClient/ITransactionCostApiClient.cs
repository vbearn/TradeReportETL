using System.Threading.Tasks;

namespace TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient
{
    public interface ITransactionCostApiClient
    {
        Task<decimal> CalculateTransactionCost(string country, decimal rate, decimal notional);
    }
}