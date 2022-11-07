using ClientApp.Models;
using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ClientApp.Controllers
{
    public class CartController : Controller
    {
        public string baseUrl = "http://localhost:5000/api/";
        public async Task<IActionResult> CustomerCart()
        {
            string token = HttpContext.Session.GetString("token");
            if (token != null)
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(baseUrl);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    HttpResponseMessage response = await Client.GetAsync("Customer/GetCustomer");
                    if (response.IsSuccessStatusCode)
                    {
                        string results = response.Content.ReadAsStringAsync().Result;
                        CustomerInfoView cus = JsonConvert.DeserializeObject<CustomerInfoView>(results);
                        ViewData["customerInfo"] = cus;
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    else
                    {
                        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                    }
                }
            }
            ViewData["cart"] = this.GetCustomerCart();
            return View("~/Views/Customer/Cart.cshtml");
        }

        private string _cartKey = Constants._cartKey;

        private List<CartItemView> GetCustomerCart()
        {
            string jsonCart = HttpContext.Session.GetString(_cartKey);
            if (jsonCart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItemView>>(jsonCart);
            }
            return new List<CartItemView>();
        }
        private void SaveCartSession(List<CartItemView> cart)
        {
            string jsoncart = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(_cartKey, jsoncart);
        }
        private void ClearCart()
        {
            HttpContext.Session.Remove(_cartKey);
        }

        //public IActionResult DeleteCart()
        //{
        //    ClearCart();
        //    return Ok("Success!");
        //}

        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            var cart = GetCustomerCart();
            var item = cart.Where(c => c.ProductID == id).FirstOrDefault();
            if (item == null)
            {
                HttpResponseMessage response = await Calculate.callGetApi($"Product/Get/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    ProductView product = JsonConvert.DeserializeObject<ProductView>(results);
                    item = new CartItemView
                    {
                        ProductID = id,
                        ProductName = product.ProductName,
                        Quantity = quantity,
                        UnitPrice = product.UnitPrice,
                    };
                    cart.Add(item);
                }
                else
                {
                    Console.WriteLine("Error Calling web API");
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
            }
            else
            {
                item.Quantity += quantity;
            }
            this.SaveCartSession(cart);
            return RedirectToAction(nameof(CustomerCart));

        }
        public IActionResult RemoveCartItem(int id)
        {
            var cart = GetCustomerCart();
            var item = cart.Where(c => c.ProductID == id).FirstOrDefault();
            if (item == null)
            {
                return RedirectToAction(nameof(CustomerCart));
            }
            cart.Remove(item);
            SaveCartSession(cart);
            return RedirectToAction(nameof(CustomerCart));
        }
        public IActionResult UpdateCartItemQuantity(int id, int quantity)
        {
            var cart = GetCustomerCart();
            var item = cart.Where(c => c.ProductID == id).FirstOrDefault();
            if (item == null)
            {
                return RedirectToAction(nameof(CustomerCart));
            }
            if (quantity > 0)
            {
                item.Quantity = quantity;
                SaveCartSession(cart);
            }
            else
            {
                cart.Remove(item);
                SaveCartSession(cart);
            }
            return RedirectToAction(nameof(CustomerCart));
        }

        public async Task<IActionResult> OrderCart(CustomerInfoView customerInfo)
        {
            DateTime now = DateTime.Now;
            OrderCartView orderCart = new OrderCartView
            {
                Address = customerInfo.Address,
                CompanyName = customerInfo.CompanyName,
                ContactName = customerInfo.ContactName,
                ContactTitle = customerInfo.ContactTitle,
                RequiredDate = new DateTime(now.Year, now.Month, now.Day + 2),
                Items = this.GetCustomerCart()
            };
            string token = HttpContext.Session.GetString("token");
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (token != null)
                {
                    Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }
                HttpResponseMessage response = await Client.PostAsJsonAsync("Cart/OrderCart", orderCart);
                if (response.IsSuccessStatusCode)
                {
                    ClearCart();
                    return RedirectToAction(nameof(CustomerCart));
                }else
                {
                    return RedirectToAction(nameof(CustomerCart));
                }
            }
        }

    }
}
