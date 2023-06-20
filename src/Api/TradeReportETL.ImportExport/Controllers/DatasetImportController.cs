using System;
using System.IO;
using System.Threading.Tasks;
using TradeReportETL.ImportExport.Services.FileUpload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NServiceBus;
using TradeReportETL.Shared.Messages;

namespace TradeReportETL.ImportExport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequestSizeLimit(2147483648)]
    public class DatasetImportController : ControllerBase
    {
        private readonly IFileUploader _fileUploader;
        private readonly ILogger<DatasetImportController> _logger;
        private readonly IMessageSession _messageSession;

        public DatasetImportController(ILogger<DatasetImportController> logger,
            IFileUploader fileUploader,
            IMessageSession messageSession
        )
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _messageSession = messageSession;
        }


        [HttpPost("")]
        public async Task<ActionResult> UploadAndStartProcessingDataset(
        )
        {
            _logger.LogInformation("Uploading the very large file into a temp path...");
            var fileName = await _fileUploader.UploadMultipartFile(HttpContext.Request, HttpContext.RequestAborted);

            _logger.LogInformation("Publishing the FileReadyForProcessEvent so that Json and Sql processors can pick it up...");

            var tradeReportId = Guid.NewGuid().ToString();
            await _messageSession.Send(new ExtractCsvFile
            {
                Id = tradeReportId,
                UploadTime = DateTime.Now,
                FileName = fileName
            }, HttpContext.RequestAborted).ConfigureAwait(false);

            _logger.LogInformation("File Import finished. {tradeReportId}", tradeReportId);


            return Ok(@$"/DatasetExport/ProgressPercentage?tradeReportId={tradeReportId}");
        }
    }
}