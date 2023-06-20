using NServiceBus;

namespace TradeReportETL.Shared.Messages
{

    public class LoadTransaction : ICommand
    {
        public const string EndpointName = "Load";
        public string TransactionId { get; set; }
    }

}
