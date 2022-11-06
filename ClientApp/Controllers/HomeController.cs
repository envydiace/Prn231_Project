using ClientApp.Models;
using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ClientApp.Controllers
{
    public class HomeController : Controller
    {
        public string baseUrl = "http://localhost:5000/api/";

        public async Task<IActionResult> Index(HomeFilterView filter)
        {
            var categories = await Calculate.GetAllCategory();
            if (categories == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            ViewData["categories"] = categories;
            HttpResponseMessage productHotResponse = await Calculate.callGetApi("Product/GetProductHot?categoryId=" + filter.CategoryId + "&pageIndex=" + filter.pageHot + "&pageSize=4");
            if (productHotResponse.IsSuccessStatusCode)
            {
                string results = productHotResponse.Content.ReadAsStringAsync().Result;
                ProductPagingView productsHot = JsonConvert.DeserializeObject<ProductPagingView>(results);
                ViewData["productsHot"] = productsHot;
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            HttpResponseMessage productsBestSaleResponse = await Calculate.callGetApi("Product/GetProductBestSale?categoryId=" + filter.CategoryId + "&pageIndex=" + filter.pageSale + "&pageSize=4");
            if (productsBestSaleResponse.IsSuccessStatusCode)
            {
                string results = productsBestSaleResponse.Content.ReadAsStringAsync().Result;
                ProductPagingView productsBestSale = JsonConvert.DeserializeObject<ProductPagingView>(results);
                ViewData["productsBestSale"] = productsBestSale;
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }



            HttpResponseMessage productNewResponse = await Calculate.callGetApi("Product/GetProductNew?categoryId=" + filter.CategoryId + "&pageIndex=" + filter.pageNew + "&pageSize=4");
            if (productHotResponse.IsSuccessStatusCode)
            {
                string results = productNewResponse.Content.ReadAsStringAsync().Result;
                ProductPagingView productsNew = JsonConvert.DeserializeObject<ProductPagingView>(results);
                ViewData["productsNew"] = productsNew;
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            ViewData["categoryId"] = filter.CategoryId;
            ViewData["pageHot"] = filter.pageHot;
            ViewData["pageSale"] = filter.pageSale;
            ViewData["pageNew"] = filter.pageNew;
            return View();
        }

        public IActionResult Filter(int id, int? pageHot = 1, int? pageSale = 1, int? pageNew = 1)
        {
            HomeFilterView homeFilterView = new HomeFilterView { CategoryId = id, pageHot = pageHot, pageSale = pageSale, pageNew = pageNew };
            return RedirectToAction("Index", "Home", homeFilterView);
        }

        public async Task< IActionResult >ProductDetail(int id)
        {
            HttpResponseMessage response = await Calculate.callGetApi($"Product/Get/{id}");
            if (response.IsSuccessStatusCode)
            {
                string results = response.Content.ReadAsStringAsync().Result;
                ProductView product = JsonConvert.DeserializeObject<ProductView>(results);
                ViewData["product"] = product;
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}