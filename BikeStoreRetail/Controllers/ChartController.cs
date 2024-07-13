using BikeStoreRetail.Models;
using BikeStoreRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStoreRetail.Controllers
{
    public class ChartController : Controller
    {
        private readonly IADODataProvider _BKDataProvider;
        public ChartController(IADODataProvider bKDataProvider)
        {
            _BKDataProvider = bKDataProvider;
        }
        public IActionResult Index()
        {
            ChartsDataModel chartsDataModel = _BKDataProvider.getChartsData();

            return View(chartsDataModel);
        }
    }
}
