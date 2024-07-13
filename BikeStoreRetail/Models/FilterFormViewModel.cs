namespace BikeStoreRetail.Models
{
    public class FilterFormViewModel
    {
        public FilterFormViewModel() { }
        public IList<StoreModel> StoreList { get; set; } = new List<StoreModel>();
        public IList<BrandModel> BrandList { get; set; } = new List<BrandModel>();
        public IList<CategoryModel> CategoryList { get; set; } = new List<CategoryModel>();
    }
}
