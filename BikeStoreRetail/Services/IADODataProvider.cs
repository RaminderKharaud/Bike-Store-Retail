using BikeStoreRetail.Models;

namespace BikeStoreRetail.Services
{
    public interface IADODataProvider
    {
        public int TotalRecords { get; set; }
        public  Task<IList<StockRecordViewModel>> GetStockRecordsAsync(StockSearchModel stockSearchModel);
        public FilterFormViewModel getFormData();
        public Task<String> getProductList(string? productName);
        public Task updateStockQuantity(int storeId, int productId, int quantity);
        public ChartsDataModel getChartsData();

    }
}
