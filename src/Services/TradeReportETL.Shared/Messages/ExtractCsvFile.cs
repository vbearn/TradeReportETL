using System;
using NServiceBus;

namespace TradeReportETL.Shared.Messages
{

    public class ExtractCsvFile : ICommand, IExtractCommand
    {
        public const string EndpointName = "Extract";
        public string Id { get; set; }
        public DateTime? UploadTime { get; set; }
        public string FileName { get; set; }
    }

}
