using System;
using TradeReportETL.Common;

namespace TradeReportETL.TransactionCost.Services.TransactionCost
{
    public class TransactionCostCalculatorService : ITransactionCostCalculatorService
    {
        public decimal CalculateTransactionCost(string country, decimal rate, decimal notional)
        {
            Guard.NotWhitespaceString(country, nameof(country));
            Guard.NotNegative(rate, nameof(rate));
            Guard.NotZero(rate, nameof(rate));


            return country.ToUpper() switch
            {
                "NL" => Math.Abs(notional * rate - notional),
                "GB" => notional * rate - notional,
                _ => throw new ArgumentOutOfRangeException(nameof(country))
            };
        }
    }
}