namespace BikeStoreRetail.Models
{
    public class StockSearchModel
    {
        private int? _pageNumber = 1;
        private int? _fetchNextRows = 50;
        public int? StoreId { get; set; }
        public int? ProductId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? Quantity { get; set; }
        public string? Orderby { get; set; }
        public bool Descending { get; set; } = false;
      
        public int? FetchNextRows {
            get
            {
                return _fetchNextRows;
            }

            set{
                _fetchNextRows=value;
            }
        }
        public int? PageNumber {
            get
            {
                return _pageNumber;
            }

            set
            {
                _pageNumber = value;
            }
        }
        public int? RowOffSet
        {
            get
            {
                return (_pageNumber - 1) * _fetchNextRows;
            }
        }
    }

}
