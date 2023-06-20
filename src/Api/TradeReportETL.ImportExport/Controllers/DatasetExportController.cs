using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common.Http;
using TradeReportETL.Pipeline.Modules.Load.Services;

namespace TradeReportETL.ImportExport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequestSizeLimit(2147483648)]
    public class DatasetExportController : ControllerBase
    {
        private readonly ITradeReportLoadService _tradeReportLoadService;
        private readonly ILogger<DatasetImportController> _logger;

        public DatasetExportController(ILogger<DatasetImportController> logger,
            ITradeReportLoadService tradeReportLoadService)
        {
            _logger = logger;
            _tradeReportLoadService = tradeReportLoadService;
        }


        [HttpGet("ProgressPercentage")]
        public async Task<ActionResult<string>> GetProgressPercentage(string tradeReportId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTradeReportProgressPercentage {tradeReportId}...", tradeReportId);

            var percentage = await _tradeReportLoadService.GetTradeReportProgressPercentage(tradeReportId, cancellationToken);

            return Ok(@$"{percentage}%");
        }

        [HttpGet("DownloadEnrichedReport")]
        public async Task DownloadEnrichedReport(string tradeReportId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DownloadEnrichedReport {tradeReportId}...", tradeReportId);

            Response.ContentType = "text/plain";
            StreamWriter streamWriter;
            await using ((streamWriter = new StreamWriter(Response.Body)).ConfigureAwait(false))
            {
                try
                {
                    await _tradeReportLoadService.GetTradeReportTransactions(tradeReportId, streamWriter, cancellationToken);
                }
                catch (NotFoundException)
                {
                    streamWriter.WriteLine("Trade Report Process Not Finished");
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
        }
    }
}