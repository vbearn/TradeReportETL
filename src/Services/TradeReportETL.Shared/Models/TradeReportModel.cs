
namespace TradeReportETL.Shared.Models
{
    public class TradeReportModel
    {
        public string Id { get; set; }
        public string[] TransactionIds { get; set; }
        public long TotalTransactionsCount { get; set; }
        public long FinishedTransactionsCount { get; set; }

        public static string GetCacheId(string tradeReportId) => "TR." + tradeReportId;
    }
}
