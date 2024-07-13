using BikeStoreRetail.Models;
using BikeStoreRetail.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BikeStoreRetail.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IADODataProvider _BKDataProvider;

        public HomeController(ILogger<HomeController> logger, IADODataProvider bKDataProvider)
        {
            _logger = logger;
            _BKDataProvider = bKDataProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            FilterFormViewModel? filterFormViewModel =  _BKDataProvider?.getFormData();
            ViewBag.Stores = filterFormViewModel?.StoreList;
            ViewBag.Brands = filterFormViewModel?.BrandList;
            ViewBag.Categories = filterFormViewModel?.CategoryList;
            
            return View();
        }

        //check form values for data filter and forward model to component view logic
        //this method returns view component to the index view
        [HttpPost]
        public IActionResult SearchStockData(StockSearchModel stockSearch)
        {
            if(!ModelState.IsValid) 
            {
                throw new Exception("Invalid Filter Form Data");
            }
           
            if (stockSearch.StoreId < 0) stockSearch.StoreId = null;
            if (stockSearch.BrandId < 0) stockSearch.BrandId = null;
            if (stockSearch.CategoryId < 0) stockSearch.CategoryId = null;
            return ViewComponent("StockGrid", stockSearch);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        //get product list from auto complete product name field
        //data comes from sql database with ADO.net
        [HttpGet]
        public async Task<IActionResult> GetProductList(string productName)
        {
            string jsonString = await _BKDataProvider.getProductList(productName);
            return new JsonResult(jsonString);
        }

        //for updating quantity in the sql database 
        [HttpGet]
        public async Task<IActionResult> UpdateQuantity(int storeId, int productId, int quantity)
        { 
            try
            {
                await _BKDataProvider.updateStockQuantity(storeId, productId, quantity);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
