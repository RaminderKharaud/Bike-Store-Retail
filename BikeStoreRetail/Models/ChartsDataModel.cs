namespace BikeStoreRetail.Models
{
    public class ChartsDataModel
    {
        public IDictionary<string, IList<double>>? TotalSalesByStore { get; set; }
        public IDictionary<string, IList<int>>? TotalOrdersByStore { get; set; }
        public IDictionary<string, double>? SalesPercentageByStore { get; set; }
        public HashSet<string>? Categories { get; set; }
        public double? TotalSalesOfAllStores { get; set; }

    }
}
