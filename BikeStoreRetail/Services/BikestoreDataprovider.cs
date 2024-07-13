using Microsoft.Data.SqlClient;
using BikeStoreRetail.Models;
using System.Data;
using Newtonsoft.Json;

namespace BikeStoreRetail.Services
{
    public class BikestoreDataprovider : IADODataProvider
    {
        string? DBConnectionString;
        public int TotalRecords { get; set; }
        public BikestoreDataprovider(IConfiguration configuration)
        {
            DBConnectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        
        public async Task<IList<StockRecordViewModel>> GetStockRecordsAsync(StockSearchModel stockSearchModel)
        {
            var records = new List<StockRecordViewModel>();
            DataSet ds = new DataSet("StockDataSet");
            
            using SqlConnection conn = new SqlConnection(DBConnectionString);
            
            await conn.OpenAsync();

            using SqlCommand sqlComm = new SqlCommand("spGetProductStock", conn);

            sqlComm.Parameters.AddWithValue("@store_id", stockSearchModel.StoreId);
            sqlComm.Parameters.AddWithValue("@product_id", stockSearchModel.ProductId);
            sqlComm.Parameters.AddWithValue("@brand_id", stockSearchModel.BrandId);
            sqlComm.Parameters.AddWithValue("@category_id", stockSearchModel.CategoryId);
            sqlComm.Parameters.AddWithValue("@orderby", stockSearchModel.Orderby);
            sqlComm.Parameters.AddWithValue("@rowOffset", stockSearchModel.RowOffSet);
            sqlComm.Parameters.AddWithValue("@fetchNextRows", stockSearchModel.FetchNextRows);

            if (stockSearchModel.Descending)
            {
                sqlComm.Parameters.AddWithValue("@desc", 1);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@desc", 0);
            }

            sqlComm.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;

            da.Fill(ds, "StockRecords");

            DataTable dt = ds.Tables["StockRecords"] ?? new DataTable();
            int.TryParse(dt.Rows[0]["total_count"].ToString(), out int totalRecords);
            TotalRecords = totalRecords;

            foreach (DataRow dr in dt.Rows)
            {
                var bkModel = new StockRecordViewModel();
                bkModel.StoreName = dr["Store Name"]?.ToString();
                bkModel.ProductName = dr["Product Name"]?.ToString();
                bkModel.Brand = dr["Brand Name"]?.ToString();
                bkModel.Category = dr["Category"]?.ToString();
                int.TryParse(dr["Quantity"]?.ToString(), out int quantity);
                bkModel.Quantity = quantity;
                int.TryParse(dr["Store Id"]?.ToString(), out int storeId);
                bkModel.StoreId = storeId;
                int.TryParse(dr["Product Id"]?.ToString(), out int productId);
                bkModel.ProductId = productId;
                records.Add(bkModel);
            }

             return records;
        }

        public FilterFormViewModel getFormData()
        {
            FilterFormViewModel filterFormViewModel = new FilterFormViewModel();
            DataSet ds = new DataSet("FilterFormDataSet");

            using (SqlConnection conn = new SqlConnection(DBConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand("spGetFilterFormData", conn);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);

                DataTable storesTable = ds.Tables[0] ?? new DataTable();
                DataTable categoryTable = ds.Tables[1] ?? new DataTable();
                DataTable brandsTable = ds.Tables[2] ?? new DataTable();

                IList<StoreModel> stores = new List<StoreModel>();
                IList<BrandModel> brands = new List<BrandModel>();
                IList<CategoryModel> categories = new List<CategoryModel>();

                foreach (DataRow dr in storesTable.Rows)
                {
                    var store = new StoreModel();
                    store.Name = dr["store_name"]?.ToString();
                    int.TryParse(dr["store_id"]?.ToString(), out int id);
                    store.Id = id;
                    stores.Add(store);
                }
                foreach(DataRow dr in categoryTable.Rows)
                {
                    var category = new CategoryModel();
                    category.Name = dr["category_name"]?.ToString();
                    int.TryParse(dr["category_id"]?.ToString(), out int id);
                    category.Id = id;
                    categories.Add(category);
                }
                foreach (DataRow dr in brandsTable.Rows)
                {
                    var brand = new BrandModel();
                    brand.Name = dr["brand_name"]?.ToString();
                    int.TryParse(dr["brand_id"]?.ToString(), out int id);
                    brand.Id = id;
                    brands.Add(brand);
                }
                filterFormViewModel.StoreList = stores;
                filterFormViewModel.CategoryList = categories;
                filterFormViewModel.BrandList = brands;

            }
            return filterFormViewModel;
        }
        public async Task<String> getProductList(string? productName)
        {
            string jsonString = "";

            DataSet ds = new DataSet("ProductDataSet");

            using SqlConnection conn = new SqlConnection(DBConnectionString);

            await conn.OpenAsync();

            using SqlCommand sqlComm = new SqlCommand("spGetProductList", conn);

            sqlComm.Parameters.AddWithValue("@product_name", productName);

            sqlComm.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;

            da.Fill(ds, "productTable");

            DataTable table = ds.Tables["productTable"] ?? new DataTable();

            jsonString = JsonConvert.SerializeObject(table);

            return jsonString;
        }
        public async Task updateStockQuantity(int storeId, int productId, int quantity)
        {
            using SqlConnection conn = new SqlConnection(DBConnectionString);

            await conn.OpenAsync();

            using SqlCommand sqlComm = new SqlCommand("spUpdateQuantity", conn);
            sqlComm.Parameters.AddWithValue("@StoreId", storeId);
            sqlComm.Parameters.AddWithValue("@ProductId", productId);
            sqlComm.Parameters.AddWithValue("@QuantityVal", quantity);
            sqlComm.CommandType = CommandType.StoredProcedure;
            await sqlComm.ExecuteNonQueryAsync();
           
        }

        public ChartsDataModel getChartsData()
        {
            var chartsData = new ChartsDataModel();
            DataSet ds = new DataSet("ChartsDataSet");

            using SqlConnection conn = new SqlConnection(DBConnectionString);
            
            SqlCommand sqlComm = new SqlCommand("spGetChartsData", conn);

            sqlComm.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;

            da.Fill(ds);

            DataTable dt = ds.Tables[0] ?? new DataTable();

            chartsData.TotalSalesByStore = new Dictionary<string, IList<double>>();
            chartsData.TotalOrdersByStore = new Dictionary<string, IList<int>>();
            chartsData.SalesPercentageByStore = new Dictionary<string,double>();
            chartsData.Categories = new HashSet<string>();
            chartsData.TotalSalesOfAllStores = 0;

            foreach (DataRow dr in dt.Rows)
            {
                double.TryParse(dr["total_sales"].ToString(), out double totalSales);
                int.TryParse(dr["total_orders"].ToString(), out int totalOrders);
                string? storeName = dr["store_name"].ToString()?.Trim();

                chartsData.Categories.Add(dr["order_year"].ToString());
                chartsData.TotalSalesOfAllStores += totalSales;

                if(storeName != null && !chartsData.TotalSalesByStore.ContainsKey(storeName))
                    chartsData.TotalSalesByStore[storeName] = new List<double>();
                if(storeName != null && !chartsData.TotalOrdersByStore.ContainsKey(storeName))
                    chartsData.TotalOrdersByStore[storeName] = new List<int>();

                chartsData.TotalSalesByStore[storeName].Add(totalSales);
                chartsData.TotalOrdersByStore[storeName].Add(totalOrders);

            }

            foreach(string key in chartsData.TotalSalesByStore.Keys)
            {
                double totalSale = 0;
                foreach(double saleValue in chartsData.TotalSalesByStore[key])
                {
                    totalSale += saleValue;
                }
                double? percent = (totalSale / chartsData.TotalSalesOfAllStores) * 100;
                chartsData.SalesPercentageByStore[key] = (double) percent;
            }

            return chartsData;
        }
    }
}
