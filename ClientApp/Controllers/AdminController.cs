using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using ClientApp.Models;
using Newtonsoft.Json;
using ClientApp.Utils;
using System.Diagnostics;

namespace ClientApp.Controllers
{
    public class AdminController : Controller
    {
        public string baseUrl = "http://localhost:5000/api/";
        public async Task<ActionResult> Product()
        {
            //using (var Client = new HttpClient())
            //{
            //    Client.BaseAddress = new Uri(baseUrl);
            //    Client.DefaultRequestHeaders.Accept.Clear();
            //    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage categoryResponse = await Client.GetAsync("Category/GetAll");
            //    if (categoryResponse.IsSuccessStatusCode)
            //    {
            //        string results = categoryResponse.Content.ReadAsStringAsync().Result;
            //        List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(results);
            //        ViewData["categories"] = categories;
            //    }
            //    else
            //    {
            //        Console.WriteLine("Error Calling web API");
            //    }
            //}
            var categories = await GetAllCategory();
            if (categories == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            ViewData["categories"] = categories;

            HttpResponseMessage productResponse = await Calculate.callApi("Product/GetAllFilter");
            if (productResponse.IsSuccessStatusCode)
            {
                string results = productResponse.Content.ReadAsStringAsync().Result;
                List<ProductView> products = JsonConvert.DeserializeObject<List<ProductView>>(results);
                ViewData["products"] = products;
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            return View("~/Views/Admin/Index.cshtml");
        }

        public async Task<ActionResult> Create()
        {
            var categories = await GetAllCategory();
            if (categories == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            ViewData["categories"] = categories;
            return View("~/Views/Admin/Create.cshtml");
        }

        public async Task<ActionResult> Update(int id)
        {
            var categories = await GetAllCategory();
            if (categories == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            ViewData["categories"] = categories;
            return View("~/Views/Admin/Update.cshtml", await GetProductById(id));
        }

        public async Task<ActionResult> CreateProduct(ProductAdd product)
        {
            if (ModelState.IsValid)
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(baseUrl);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await Client.PostAsJsonAsync("Product/Create", product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    {
                        return View("~/Views/Home/Index.cshtml");
                    }
                    else
                    {
                        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                    }
                }
            }
            else
            {
                var categories = await GetAllCategory();
                if (categories == null)
                {
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
                ViewData["categories"] = categories;
                return View("~/Views/Admin/Create.cshtml");
            }
        }

        public async Task<ActionResult> UpdateProduct(ProductEdit product)
        {
            if (ModelState.IsValid)
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(baseUrl);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await Client.PutAsJsonAsync("Product/Update", product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    {
                        return View("~/Views/Home/Index.cshtml");
                    }
                    else
                    {
                        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                    }
                }
            }
            else
            {
                var categories = await GetAllCategory();
                if (categories == null)
                {
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
                ViewData["categories"] = categories;
                return View("~/Views/Admin/Update.cshtml");
            }
        }

        public async Task<ActionResult> DeleteProduct(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Client.DefaultRequestHeaders.Add("Authentication", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI3NzVlOGRkYy00ZGU4LTRkODMtOThmNi0wOWM3NTA3ZDExNDkiLCJpYXQiOiIxMS8yLzIwMjIgMzo0NDoyNSBQTSIsIkFjY291bnRJZCI6IjMiLCJQYXNzd29yZCI6WyIxMjMiLCIxMjMiXSwiRW1haWwiOiJlbXAxQGZwdC5lZHUudm4iLCJDdXN0b21lcklkIjoiIiwiRW1wbG95ZWVJZCI6IjUiLCJSb2xlIjoiMSIsImV4cCI6MTY2NzQwMzg3NSwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.AF31xHPcjcci6mh5FGbKLTzzSd6S7ug-9hnPVoB-2jQ");
                HttpResponseMessage response = await Client.DeleteAsync($"Product/Delete/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Product));
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return View("~/Views/Home/Index.cshtml");
                }
                else
                {
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
            }
        }


        private async Task<List<CategoryView>> GetAllCategory()
        {
            HttpResponseMessage categoryResponse = await Calculate.callApi("Category/GetAll");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string results = categoryResponse.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<CategoryView>>(results);
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return null;
            }
        }

        private async Task<ProductEdit> GetProductById(int id)
        {
            HttpResponseMessage productResponse = await Calculate.callApi($"Product/Get/{id}");
            if (productResponse.IsSuccessStatusCode)
            {
                string results = productResponse.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ProductEdit>(results);
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return null;
            }
        }


        // GET: Admin
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: Admin/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Admin/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Admin/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Admin/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Admin/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Admin/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Admin/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
