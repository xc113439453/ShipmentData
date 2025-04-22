using ShipmentData.Interfaces;
using ShipmentData.Utils;

namespace ShipmentData.Models
{
    internal class DenaliFactory : IProductFactory
    {
        public List<IProductModel> ProcessSummaryData(List<IProductModel> dataList, List<ISummaryModel> summaryList)
        {
            var productList = dataList.Cast<DenaliModel>().ToList();
            productList.ForEach(data =>
            {
                ExcelUtil.GetSummaryData(data, summaryList.Cast<DenaliSummaryModel>().ToList());
            });

            return productList.Cast<IProductModel>().ToList();
        }

        public List<IProductModel> ReadShipmentDataFile(string filePath)
        {
            return ExcelUtil.ReadShipmentDataFile<DenaliModel>(filePath)
                 .Cast<IProductModel>()
                 .ToList();
        }

        public List<ISummaryModel> ReadSummaryFile(string filePath)
        {
            return ExcelUtil.ReadSummaryFile<DenaliSummaryModel>(filePath)
                .Cast<ISummaryModel>()
                .ToList();
        }

        public void WriteBackToShipmentDataFile(string filePath, List<IProductModel> dataList)
        {
            ExcelUtil.WriteBackToShipmentDataFile(filePath, dataList.Cast<DenaliModel>().ToList());
        }
    }
}
