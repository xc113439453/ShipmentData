using ShipmentData.Interfaces;
using ShipmentData.Utils;

namespace ShipmentData.Models
{
    internal class DenaliV3Factory : IProductFactory
    {
        public List<IProductModel> ProcessSummaryData(List<IProductModel> dataList, List<ISummaryModel> summaryList)
        {
            var productList = dataList.Cast<DenaliV3Model>().ToList();
            productList.AsParallel().ForAll(data =>
            {
                ExcelUtil.GetSummaryData(data, summaryList.Cast<DenaliV3SummaryModel>().ToList());
            });

            return productList.Cast<IProductModel>().ToList();
        }

        public List<IProductModel> ReadShipmentDataFile(string filePath)
        {
            return ExcelUtil.ReadShipmentDataFile<DenaliV3Model>(filePath)
                 .Cast<IProductModel>()
                 .ToList();
        }

        public List<ISummaryModel> ReadSummaryFile(string filePath)
        {
            List<DenaliV3SummaryModel> list = CSVUtil.ReadSummaryFile<DenaliV3SummaryModel>(filePath);
            list.AsParallel().ForAll(data =>
            {
                data.Channel = $"CH{data.Channel}";
            });
            return list.Cast<ISummaryModel>().ToList();
        }

        public void WriteBackToShipmentDataFile(string filePath, List<IProductModel> dataList)
        {
            ExcelUtil.WriteBackToShipmentDataFile(filePath, dataList.Cast<DenaliV3Model>().ToList());
        }
    }
}
