using System.Linq;
using ShipmentData.Interfaces;
using ShipmentData.Utils;

namespace ShipmentData.Models
{
    internal class WeserFactory : IProductFactory
    {
        public List<IProductModel> ProcessSummaryData(List<IProductModel> dataList, List<ISummaryModel> summaryList)
        {
            var productList = dataList.Cast<WeserModel>().ToList();
            productList.ForEach(data =>
            {
                ExcelUtil.GetSummaryData(data, summaryList.Cast<WeserSummaryModel>().ToList());
            });

            return productList.Cast<IProductModel>().ToList();
        }

        public List<IProductModel> ReadShipmentDataFile(string filePath)
        {
            return ExcelUtil.ReadShipmentDataFile<WeserModel>(filePath)
                 .Cast<IProductModel>()
                 .ToList();
        }

        public List<ISummaryModel> ReadSummaryFile(string filePath)
        {
            return ExcelUtil.ReadSummaryFile<WeserSummaryModel>(filePath)
                .Cast<ISummaryModel>()
                .ToList();
        }

        public void WriteBackToShipmentDataFile(string filePath, List<IProductModel> dataList)
        {

            ExcelUtil.WriteBackToShipmentDataFile(filePath, dataList.Cast<WeserModel>().ToList());
        }
    }
}
