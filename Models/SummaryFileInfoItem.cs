namespace ShipmentData.Models;

public class SummaryFileInfoItem
{
    public string FileName { get; set; }   // 只显示文件名
    public string FilePath { get; set; }   // 完整路径（可用于后续打开等操作）

    public SummaryFileInfoItem(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);  // 自动提取文件名
    }

    // 可选：重写 ToString，便于调试
    public override string ToString()
    {
        return $"{FileName} ({FilePath})";
    }
}
