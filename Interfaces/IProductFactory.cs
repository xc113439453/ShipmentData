using ShipmentData.Models;

namespace ShipmentData.Interfaces
{
    internal interface IProductFactory
    {
        List<IProductModel> ReadShipmentDataFile(string filePath);
        List<ISummaryModel> ReadSummaryFile(List<SummaryFileInfoItem> files);
        void WriteBackToShipmentDataFile(string filePath, List<IProductModel> dataList);
        List<IProductModel> ProcessSummaryData(List<IProductModel> dataList, List<string> snList, List<ISummaryModel> summaryList);

        List<string> ReadSNListFile(string filePath);
    }
}
