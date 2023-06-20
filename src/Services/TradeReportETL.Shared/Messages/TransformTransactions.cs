using NServiceBus;

namespace TradeReportETL.Shared.Messages
{

    public class TransformTransactions : ICommand
    {
        public const string EndpointName = "Transform";
        public string[] TransactionIds { get; set; }
    }

}
