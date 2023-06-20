namespace TradeReportETL.TransactionCost.Services.TransactionCost
{
    public interface ITransactionCostCalculatorService
    {
        decimal CalculateTransactionCost(string country, decimal rate, decimal notional);
    }
}