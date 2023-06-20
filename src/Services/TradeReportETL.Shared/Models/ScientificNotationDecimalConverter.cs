using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;
using System.Globalization;
using TradeReportETL.Common;
using System.Linq;

namespace TradeReportETL.Shared.Models
{
    public class ScientificNotationDecimalConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            Guard.NotNull(text, memberMapData.Names.FirstOrDefault());
            return decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }

}
