namespace ShipmentData.Interfaces
{
    internal interface IProductFactory
    {
        List<IProductModel> ReadShipmentDataFile(string filePath);
        List<ISummaryModel> ReadSummaryFile(string filePath);
        void WriteBackToShipmentDataFile(string filePath, List<IProductModel> dataList);
        List<IProductModel> ProcessSummaryData(List<IProductModel> dataList, List<ISummaryModel> summaryList);
    }
}
