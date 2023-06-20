using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Pipeline.Modules.Extract.Services.Csv;

public class CsvTradeReportParser
{
    public static async IAsyncEnumerable<TradeReportTransactionModel> ParseCsv(StreamReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        var csvRows = csvReader.GetRecordsAsync<TradeReportTransactionModel>(cancellationToken);
        if (csvRows is null)
        {
            throw new ArgumentException("Error parsing CSV File.");
        }

        var rowEnumerator = csvRows.GetAsyncEnumerator(cancellationToken);
        for (var hasTransactions = true; hasTransactions;)
        {
            try
            {
                hasTransactions = await rowEnumerator.MoveNextAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not parse csv stream correctly", ex);
            }

            if (hasTransactions)
            {
                yield return rowEnumerator.Current;
            }
        }
    }
}