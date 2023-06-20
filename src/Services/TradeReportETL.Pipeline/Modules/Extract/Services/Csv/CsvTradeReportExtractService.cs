using FluentStorage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Common;
using TradeReportETL.Pipeline.Modules.Extract.Interfaces;
using TradeReportETL.Shared.Messages;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Extract.Services.Csv
{
    public class CsvTradeReportExtractService : IExtractService
    {
        private readonly ILogger<CsvTradeReportExtractService> _logger;
        private readonly IConfiguration _configuration;


        public CsvTradeReportExtractService(
            ILogger<CsvTradeReportExtractService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IAsyncEnumerable<TradeReportTransactionModel>> ExtractFile(IExtractCommand message,
            CancellationToken cancellationToken)
        {
            Guard.NotWhitespaceString(message.FileName, nameof(message.FileName));

            _logger.LogInformation("Start reading CSV chunk by chunk from file {PathOnDisk} ...", message?.FileName);

            StreamReader streamReader;
            try
            {
                IBlobStorage storage = FluentStorageHelpers.CreateBlobStorage(
                          _configuration.GetValue<string>("Storage:ConnectionString"));

                var stream = await storage.OpenReadAsync(message.FileName);

                streamReader = new StreamReader(stream);

            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Cannot read from BlobStorage with fileName in {message.FileName}. Make sure the Blob Storage ConnectionString is set correctly in appSettings.", e);
            }

            var transactions = CsvTradeReportParser.ParseCsv(streamReader, cancellationToken);

            return transactions;
        }
    }
}