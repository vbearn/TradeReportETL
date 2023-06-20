using System;
using NServiceBus;

namespace TradeReportETL.Shared.Events
{

    public class TradeReportDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string TradeReportsCollectionName { get; set; } = null!;
        public string TradeReportTransactionsCollectionName { get; set; } = null!;
    }

}
