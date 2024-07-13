using BikeStoreRetail.Models;
using BikeStoreRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStoreRetail.Components
{
    public class StockGridViewComponent : ViewComponent
    {
        private readonly IADODataProvider _BKDataProvider;

        public StockGridViewComponent(IADODataProvider bKDataProvider)
        {
            _BKDataProvider = bKDataProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync(StockSearchModel? stockSearchModel = null)
        {
            if(stockSearchModel == null)
            {
                stockSearchModel=new StockSearchModel();
            }
            IList<StockRecordViewModel>? list = await _BKDataProvider.GetStockRecordsAsync(stockSearchModel);
            ViewBag.PageNumber = stockSearchModel.PageNumber;
            ViewBag.TotalPages = (int) Math.Ceiling(((double)_BKDataProvider.TotalRecords) / ((double)stockSearchModel?.FetchNextRows));
            return View(list);
        }
    }
}
