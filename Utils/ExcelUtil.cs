using System.ComponentModel;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ShipmentData.Interfaces;

namespace ShipmentData.Utils;

public class ExcelUtil
{
    // 获取属性和表头映射
    private static List<(PropertyInfo Property, string ColumnName)> GetPropertyMappings<T>()
    {
        return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(prop => (Property: prop, ColumnName: prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName))
            .Where(x => x.ColumnName != null)
            .ToList();
    }

    //查找表头对应的列索引并验证
    private static Dictionary<string, int> GetColumnIndexMap(ExcelWorksheet worksheet, int headerRow,
        List<(PropertyInfo Property, string ColumnName)> mappings)
    {
        var columnIndexMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            string header = worksheet.Cells[headerRow, col].Text.Trim();
            var mapping = mappings.FirstOrDefault(m => m.ColumnName == header);
            if (mapping.Property != null && !columnIndexMap.ContainsKey(mapping.Property.Name))
            {
                columnIndexMap[mapping.Property.Name] = col;
            }
        }

        // 验证表头
        var missingColumns = mappings
            .Where(m => !columnIndexMap.ContainsKey(m.Property.Name))
            .Select(m => m.ColumnName);
        if (missingColumns.Any())
        {
            throw new InvalidOperationException($"缺少表头: {string.Join(", ", missingColumns)}");
        }

        return columnIndexMap;
    }

    internal static List<T> ReadShipmentDataFile<T>(string filePath) where T : IProductModel, new()
    {
        var result = new List<T>();

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];

            // 读取元数据（可选）
            string partNumber = worksheet.Cells["B3"].Text;
            string productionQty = worksheet.Cells["B5"].Text;
            string productionDate = worksheet.Cells["B6"].Text;
            Console.WriteLine($"Part Number: {partNumber}, Qty: {productionQty}, Date: {productionDate}");

            // 表头在第 9 行，数据从第 12 行开始
            int headerRow = 9;
            int startRow = 12;
            int headerStartColumn = 2;

            // 使用 HashSet 存储通道值去重值
            HashSet<string> uniqueChannels = new HashSet<string>();


            for (int row = startRow; row <= worksheet.Dimension.Rows; row++)
            {
                var cellValue = worksheet.Cells[row, 4].Value?.ToString();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    uniqueChannels.Add(cellValue.ToString().ToUpper());
                }
            }


            //string[] channels = { "CH1", "CH2", "CH3", "CH4" };
            string[] channels = uniqueChannels.ToArray();

            // 获取属性映射和列索引
            var propertyMappings = GetPropertyMappings<T>();

            // add 20250627
            // 可能存在出货模板减少列的情况
            propertyMappings = FilterByShipmentTemplate(worksheet, headerRow, headerStartColumn, propertyMappings);

            var columnIndexMap = GetColumnIndexMap(worksheet, headerRow, propertyMappings);

            // 遍历数据行
            for (int row = startRow; !string.IsNullOrEmpty(worksheet.Cells[row, columnIndexMap["No"]].Text); row++)
            {
                string sn = worksheet.Cells[row, columnIndexMap["SN"]].Text;
                string no = worksheet.Cells[row, columnIndexMap["No"]].Text;

                // 为每个通道创建记录
                for (int i = 0; i < channels.Length; i++)
                {
                    var data = new T();
                    foreach (var mapping in propertyMappings)
                    {
                        var prop = mapping.Property;
                        int colIndex = columnIndexMap[prop.Name];
                        string cellValue = worksheet.Cells[row + i, colIndex].Text;

                        // 根据属性类型设置值
                        try
                        {
                            if (prop.PropertyType == typeof(int))
                                prop.SetValue(data, int.Parse(cellValue));
                            else if (prop.PropertyType == typeof(string))
                                prop.SetValue(data, cellValue);
                            else if (prop.PropertyType == typeof(decimal?))
                                prop.SetValue(data, ParseDecimal(cellValue));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"解析错误: 行 {row + i}, 列 {colIndex}, 值 {cellValue}, 属性 {prop.Name}: {ex.Message}");
                        }
                    }
                    data.SN = sn;
                    data.No = int.Parse(no);
                    data.Channel = channels[i]; // 手动设置通道
                    result.Add(data);
                }

                row += channels.Length - 1; // 跳过已处理的通道行
            }
        }

        return result;
    }

    private static List<(PropertyInfo Property, string ColumnName)> FilterByShipmentTemplate(ExcelWorksheet worksheet, int headerRow, int headerStartColumn, List<(PropertyInfo Property, string ColumnName)> propertyMappings)
    {
        List<string> shipmentHeadColumns = GetRangeValuesAsList(worksheet, worksheet.Cells[headerRow, headerStartColumn, headerRow, worksheet.Dimension.End.Column].Address);
        return propertyMappings = propertyMappings
            .Where(mapping => shipmentHeadColumns.Contains(mapping.ColumnName))
            .ToList();
    }

    private static decimal? ParseDecimal(string text)
    {
        if (decimal.TryParse(text, out decimal value))
            return value;
        return null;
    }

    internal static void WriteBackToShipmentDataFile<T>(string filePath, List<T> dataList, string amountCell) where T : IProductModel
    {
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];

            // 表头和数据行设置
            int headerRow = 9;
            int startRow = 12;
            int headerStartColumn = 2;

            // 获取属性映射和列索引
            var propertyMappings = GetPropertyMappings<T>();

            // add 20250627
            // 可能存在出货模板减少列的情况
            propertyMappings = FilterByShipmentTemplate(worksheet, headerRow, headerStartColumn, propertyMappings);

            var columnIndexMap = GetColumnIndexMap(worksheet, headerRow, propertyMappings);

            // 清空数据区域（从第 12 行开始）
            int rowCount = worksheet.Dimension.End.Row - startRow + 1;
            if (rowCount > 0)
            {
                var range = worksheet.Cells[startRow, 1, startRow + rowCount - 1, worksheet.Dimension.End.Column];
                foreach (var cell in range)
                {
                    cell.Value = null; // 仅清除值，保留样式
                }
            }

            // 按 SN 分组并写入数据
            int currentRow = startRow;
            var groupedData = dataList.GroupBy(d => d.SN).OrderBy(g => g.First().No);
            foreach (var group in groupedData)
            {
                string sn = group.Key;
                var channels = group.OrderBy(d => d.Channel); // 假设按 CH1, CH2, CH3, CH4 排序
                int no = channels.Select(t => t.No).FirstOrDefault();
                foreach (var data in channels)
                {
                    foreach (var mapping in propertyMappings)
                    {
                        var prop = mapping.Property;
                        int colIndex = columnIndexMap[prop.Name];
                        var value = prop.GetValue(data);

                        // 写入单元格
                        worksheet.Cells[currentRow, colIndex].Value = value;
                        worksheet.Cells[currentRow, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    currentRow++;
                }

                // 设置 SN 合并单元格
                int snColIndex = columnIndexMap["SN"];
                int noColIndex = columnIndexMap["No"];
                worksheet.Cells[currentRow - channels.Count(), snColIndex, currentRow - 1, snColIndex].Merge = true;
                worksheet.Cells[currentRow - channels.Count(), snColIndex].Value = sn;

                worksheet.Cells[currentRow - channels.Count(), noColIndex, currentRow - 1, noColIndex].Merge = true;
                worksheet.Cells[currentRow - channels.Count(), noColIndex].Value = no;
            }

            worksheet.Cells[amountCell].Value = $"{groupedData.Count()}pcs";

            // 或者保留打印区域，但强制让它包含所有数据（不推荐，容易出错）
            var lastRow = worksheet.Dimension?.End.Row ?? 1000;
            var lastCol = worksheet.Dimension?.End.Column ?? 26;
            worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, lastRow + 50, lastCol];  // 预留一些空行

            // ==================== 保存对话框 ====================
            using var sfd = new SaveFileDialog
            {
                Filter = "Excel 文件|*.xlsx",
                FileName = $"出货报告.xlsx",
                Title = "保存结果"
            };
           

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                package.SaveAs(new FileInfo(sfd.FileName));

                // 自动打开文件所在目录并选中
                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + sfd.FileName + "\"");
            }
        }
    }


    internal static List<T> ReadSummaryFile<T>(string filePath) where T : new()
    {
        var result = new List<T>();
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            // 表头在第 1 行，数据从第 4 行开始
            int headerRow = 1;
            int startRow = 4;

            // 获取属性映射和列索引
            var propertyMappings = GetPropertyMappings<T>();
            var columnIndexMap = GetColumnIndexMap(worksheet, headerRow, propertyMappings);

            // 遍历数据行
            for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
            {
                // 检查是否是空行（以第一个映射的列为准）
                if (string.IsNullOrEmpty(worksheet.Cells[row, columnIndexMap[propertyMappings[0].Property.Name]].Text))
                    break;

                var data = new T();
                foreach (var mapping in propertyMappings)
                {

                    var prop = mapping.Property;
                    int colIndex = columnIndexMap[prop.Name];
                    string cellValue = worksheet.Cells[row, colIndex].Text;

                    // 根据属性类型设置值
                    try
                    {
                        if (prop.PropertyType == typeof(int))
                            prop.SetValue(data, int.Parse(cellValue));
                        else if (prop.PropertyType == typeof(string))
                            prop.SetValue(data, cellValue);
                        else if (prop.PropertyType == typeof(decimal?))
                            prop.SetValue(data, ParseDecimal(cellValue));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"解析错误: 行 {row}, 列 {colIndex}, 值 {cellValue}, 属性 {prop.Name}: {ex.Message}");
                    }
                }
                result.Add(data);
            }

        }
        return result;
    }

    internal static List<string> ReadSNListFile(string filePath)
    {
        var snList = new List<string>();
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets["报检SN"];
            // 假设 SN 列在第 1 列，数据从第 1 行开始
            int startRow = 1;
            for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
            {
                string sn = worksheet.Cells[row, 1].Text;
                if (!string.IsNullOrEmpty(sn))
                {
                    snList.Add(sn.Trim());
                }
            }
        }
        return snList;
    }

    internal static void GetSummaryData<TProduct, TSummary>(TProduct product, List<TSummary> list)
            where TProduct : IProductModel
            where TSummary : ISummaryModel
    {
        var snRecord = list.Where(summary => summary.SN == product.SN && $"{summary.Channel.Replace("CH", "")}" == product.Channel.Replace("CH", "")).FirstOrDefault();
        if (snRecord == null)
        {
            throw new Exception($"未找到SN：{product.SN}, Channel:{product.Channel}的Summary数据!");
        }
        snRecord.Channel = $"CH{snRecord.Channel.Replace("CH", "")}";
        var sourceProperties = typeof(TSummary).GetProperties().ToDictionary(prop => prop.Name, prop => prop);
        var targetProperties = typeof(TProduct).GetProperties();

        foreach (var targetProp in targetProperties)
        {
            // 检查 T2 是否有同名属性
            if (sourceProperties.TryGetValue(targetProp.Name, out var sourceProp))
            {
                // 确保源属性可读，目标属性可写，且类型兼容
                if (sourceProp.CanRead && targetProp.CanWrite && sourceProp.PropertyType == targetProp.PropertyType)
                {
                    // 获取源属性的值
                    var value = sourceProp.GetValue(snRecord);
                    // 赋值给目标属性
                    targetProp.SetValue(product, value);
                }
            }
        }


    }

    public static List<string> GetRangeValuesAsList(ExcelWorksheet worksheet, string address)
    {
        var range = worksheet.Cells[address];
        var rangeValues = range.Value as object[,];
        var valueList = new List<string>();
        if (rangeValues != null)
        {
            // 遍历范围值（单行，第二维是列）
            valueList = rangeValues
                .Cast<object>() // 展平二维数组
                .Where(v => v != null && !string.IsNullOrWhiteSpace(v.ToString()))
                .Select(v => Convert.ToString(v).Trim())
                .ToList();
        }

        return valueList;
    }
}

