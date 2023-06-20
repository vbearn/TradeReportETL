using CsvHelper.Configuration.Attributes;
using System;

namespace TradeReportETL.Shared.Models
{

    public class TradeReportTransactionModel
    {
        /// Universal Transaction Identifier
        [Name("transaction_uti")]
        public string TransactionUti { get; set; }

        /// International Securities Identification Number
        [Name("isin")]
        public string Isin { get; set; }

        [Name("notional")]
        [TypeConverter(typeof(ScientificNotationDecimalConverter))]
        public decimal Notional { get; set; }
      
        [Name("notional_currency")]
        public string NotionalCurrency { get; set; }
       
        [Name("transaction_type")]
        public string TransactionType { get; set; }
      
        [Name("transaction_datetime")]
        public DateTimeOffset TransactionDateTime { get; set; }
       
        [Name("rate")]
        public decimal Rate { get; set; }

        /// Legal Entity Identifier
        [Name("lei")]
        public string Lei { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string Id { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string TradeReportId { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string LegalName { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string Bic { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string Country { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public decimal TransactionCost { get; set; }

        public static string GetCacheId(string tradeTransactionReportId) => "TTR." + tradeTransactionReportId;

    }


}
