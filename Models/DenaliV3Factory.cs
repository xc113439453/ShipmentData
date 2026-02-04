using ShipmentData.Interfaces;
using ShipmentData.Utils;

namespace ShipmentData.Models
{
    public abstract class DenaliV3Factory<TSummary, TProduct> : IProductFactory
        where TSummary : ISummaryModel, new()
        where TProduct : BaseProductModel, IProductModel, new()
    {
        public abstract string AmountCellAddress { get; }
        public List<IProductModel> ProcessSummaryData(List<IProductModel> dataList, List<string> snList, List<ISummaryModel> summaryList)
        {
            int i = 1;
            foreach (var sn in snList)
            {
                foreach (var channel in summaryList.Where(t => t.SN == sn).Select(t => t.Channel))
                {
                    dataList.Add(new TProduct { No = i, SN = sn, Channel = channel });
                }
                i++;
            }
            var productList = dataList.Cast<TProduct>().ToList();
            //productList.AsParallel().ForAll(data =>
            //{
            //    ExcelUtil.GetSummaryData(data, summaryList.Cast<TSummary>().ToList());
            //});
            foreach (var data in productList)
            {
                ExcelUtil.GetSummaryData(data, summaryList.Cast<TSummary>().ToList());
            }

            return productList.Cast<IProductModel>().ToList();
        }

        public List<IProductModel> ReadShipmentDataFile(string filePath)
        {
            //return ExcelUtil.ReadShipmentDataFile<T2>(filePath)
            //     .Cast<IProductModel>()
            //     .ToList();
            return new List<TProduct>().Cast<IProductModel>().ToList();
        }

        public List<string> ReadSNListFile(string filePath)
        {
            return ExcelUtil.ReadSNListFile(filePath);
        }

        public abstract List<ISummaryModel> ReadSummaryFile(List<SummaryFileInfoItem> files);


        public void WriteBackToShipmentDataFile(string filePath, List<IProductModel> dataList)
        {
            ExcelUtil.WriteBackToShipmentDataFile(filePath, dataList.Cast<TProduct>().ToList(), AmountCellAddress);
        }
    }
}
