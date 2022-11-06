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

        public async Task<IActionResult> Index()
        {
            var categories = await Calculate.GetAllCategory();
            if (categories == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            ViewData["categories"] = categories;
            HttpResponseMessage productHotResponse = await Calculate.callGetApi("Product/GetProductBestSale?pageIndex=1&pageSize=4");
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