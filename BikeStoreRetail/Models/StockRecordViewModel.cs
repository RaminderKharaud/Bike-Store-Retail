namespace BikeStoreRetail.Models
{
    public class StockRecordViewModel
    {
        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public int? Quantity { get; set; }
    }
}
