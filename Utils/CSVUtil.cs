using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace ShipmentData.Utils;

public class CSVUtil
{
    public static List<T> ReadSummaryFile<T>(string filePath) where T : new()
    {


        var records = new List<T>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, // 假设原始CSV有列头
            Delimiter = "," // 默认分隔符 
        };
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            // 读取列头
            csv.Read();
            csv.ReadHeader();
            records = csv.GetRecords<T>().ToList();
        }
        return records;
    }
}
