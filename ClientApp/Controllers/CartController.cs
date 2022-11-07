using ClientApp.Models;
using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ClientApp.Controllers
{
    public class CartController : Controller
    {
        public IActionResult CustomerCart()
        {
            ViewData["cart"] = this.GetCustomerCart();
            return View("~/Views/Customer/Cart.cshtml");
        }

        private string _cartKey = "Cart";

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

        //[HttpDelete("[action]")]
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

        //[HttpPost("[action]")]
        //public async Task<IActionResult> OrderCartAsync([FromForm] CustomerOrderInfoDTO custInfo)
        //{
        //    var listOrderDetails = this.GetCustomerCart();
        //    var header = Request.Headers["Authorization"];
        //    string cusId = "";

        //    if (header.Count > 0)
        //    {
        //        var token = header[0].Split(" ")[1];
        //        var handler = new JwtSecurityTokenHandler();
        //        var jwt = handler.ReadJwtToken(token);
        //        cusId = jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
        //    }
        //    else
        //    {
        //        Random random = new Random();
        //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //        bool existed = true;
        //        while (existed)
        //        {
        //            cusId = new string(Enumerable.Repeat(chars, 5)
        //                .Select(s => s[random.Next(s.Length)]).ToArray());
        //            Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId.Equals(cusId));
        //            if (customer == null)
        //            {
        //                existed = false;
        //            }
        //        }
        //        Customer c = new Customer
        //        {
        //            CustomerId = cusId,
        //            CompanyName = custInfo.CompanyName != null ? custInfo.CompanyName : "",
        //            ContactName = custInfo.ContactName,
        //            ContactTitle = custInfo.ContactTitle,
        //            Address = custInfo.Address
        //        };
        //        return Ok(c);
        //    }
        //    //Order order = new Order
        //    //{
        //    //    OrderDate = DateTime.Now,
        //    //    RequiredDate = custInfo.RequiredDate
        //    //};
        //    return Ok(cusId);
        //}
    }
}
