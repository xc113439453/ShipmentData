using ShipmentData.Interfaces;
using ShipmentData.Utils;

namespace ShipmentData.Models;

public class DenaliV3PICFactory<TSummary, TProduct> : DenaliV3Factory<TSummary, TProduct>
    where TSummary : ISummaryModel, new()
    where TProduct : BaseProductModel, IProductModel, new()
{
    public override List<ISummaryModel> ReadSummaryFile(List<SummaryFileInfoItem> files)
    {
        List<TSummary> list = new();
        foreach (var file in files)
        {
            var tempList = ExcelUtil.ReadSummaryFile<TSummary>(file.FilePath);
            list.AddRange(tempList);
        }
        list.AsParallel().ForAll(data =>
        {
            if (!data.Channel.Contains("CH"))
                data.Channel = $"CH{data.Channel}";
        });
        return list.Cast<ISummaryModel>().ToList();
    }
}
