using System;

namespace TradeReportETL.Shared.Messages
{
    public interface IExtractCommand
    {
        public string Id { get; set; }
        public DateTime? UploadTime { get; set; }
        public string FileName { get; set; }
    }

}
