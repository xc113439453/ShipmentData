using ShipmentData.Interfaces;
using ShipmentData.Utils;

namespace ShipmentData.Models;

public class DenaliV3WaferFactory<TSummary, TProduct> : DenaliV3Factory<TSummary, TProduct>
    where TSummary : ISummaryModel, new()
    where TProduct : BaseProductModel, IProductModel, new()
{
    public override List<ISummaryModel> ReadSummaryFile(List<SummaryFileInfoItem> files)
    {
        List<TSummary> list = new();
        foreach (var file in files)
        {
            var tempList = CSVUtil.ReadSummaryFile<TSummary>(file.FilePath);
            list.AddRange(tempList);
        }
        return list.Cast<ISummaryModel>().ToList();
    }
}