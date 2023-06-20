using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradeReportETL.TransactionCost.Services.TransactionCost;

namespace TradeReportETL.TransactionCost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionCostController : ControllerBase
    {
        private readonly ITransactionCostCalculatorService _transactionCostCalculatorService;
        private readonly ILogger<TransactionCostController> _logger;

        public TransactionCostController(ILogger<TransactionCostController> logger,
            ITransactionCostCalculatorService transactionCostCalculatorService)
        {
            _logger = logger;
            _transactionCostCalculatorService = transactionCostCalculatorService;
        }


        [HttpGet("")]
        public ActionResult CalculateTransactionCost(
            [FromQuery] string country,
            [FromQuery] decimal rate,
            [FromQuery] decimal notional)
        {
            _logger.LogInformation("CalculateTransactionCost...");

            var cost = _transactionCostCalculatorService.CalculateTransactionCost(
                country, rate, notional);

            _logger.LogInformation("Finished CalculateTransactionCost.");

            return Ok(cost);
        }
    }
}