using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Prm231_Project.DTO;
using Prm231_Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Web;

namespace Prm231_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;
        public CartController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
            testCart.Add(new CartItemDTO
            {
                ProductID = 1,
                ProductName = "test 1",
                Quantity = 2,
                UnitPrice = 100
            });
            testCart.Add(new CartItemDTO
            {
                ProductID = 2,
                ProductName = "test 2",
                Quantity = 1,
                UnitPrice = 200
            });
            testCart.Add(new CartItemDTO
            {
                ProductID = 3,
                ProductName = "test 3",
                Quantity = 3,
                UnitPrice = 300
            });
        }

        private List<CartItemDTO> testCart = new List<CartItemDTO>();

        private string _cartKey = "Cart";

        private List<CartItemDTO> GetCustomerCart()
        {
            //string jsonCart = Request.Cookies[_cartKey];
            string jsonCart = HttpContext.Session.GetString(_cartKey);
            if (jsonCart != null)
            {
                return JsonSerializer.Deserialize<List<CartItemDTO>>(jsonCart);
            }
            return new List<CartItemDTO>();
            //return testCart;
        }
        private void SaveCartCookie(List<CartItemDTO> cart)
        {
            string jsoncart = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(_cartKey, jsoncart);
            //Response.Cookies.Append(_cartKey, jsoncart);
        }
        private void ClearCart()
        {
            HttpContext.Session.Clear();
            //Response.Cookies.Delete(_cartKey);
        }

        [HttpGet("[action]")]
        public ActionResult<List<CartItemDTO>> GetCart()
        {
            var result = GetCustomerCart();
            return Ok(result);
        }

        [HttpDelete("[action]")]
        public IActionResult DeleteCart()
        {
            ClearCart();
            return Ok("Success!");
        }
        [HttpPost("[action]")]
        public IActionResult AddToCart(CartItemDTO cartItem)
        {
            var cart = GetCustomerCart();
            var item = cart.Where(c => c.ProductID == cartItem.ProductID).FirstOrDefault();
            if (item == null)
            {
                var product = _context.Products.Where(p => p.ProductId == cartItem.ProductID).FirstOrDefault();
                item = new CartItemDTO
                {
                    ProductID = cartItem.ProductID,
                    ProductName = product.ProductName,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.UnitPrice
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += cartItem.Quantity;
            }
            SaveCartCookie(cart);
            return CreatedAtAction("GetCart", item);

        }
        [HttpDelete("[action]")]
        public IActionResult RemoveCartItem(int id)
        {
            var cart = GetCustomerCart();
            var item = cart.Where(c => c.ProductID == id).FirstOrDefault();
            if (item == null)
            {
                return BadRequest("Removed item does not existed!");
            }
            cart.Remove(item);
            SaveCartCookie(cart);
            return Ok("Remove Success!");
        }

        [HttpPut("[action]")]
        public IActionResult UpdateCartItemQuantity(int id, int quantity)
        {
            var cart = GetCustomerCart();
            var item = cart.Where(c => c.ProductID == id).FirstOrDefault();
            if (item == null)
            {
                return BadRequest("Item does not existed!");
            }
            if (quantity > 0)
            {
                item.Quantity = quantity;
                SaveCartCookie(cart);
            }
            else
            {
                cart.Remove(item);
                SaveCartCookie(cart);
            }
            return Ok("Update Success!");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OrderCartAsync(CustomerOrderInfoDTO custInfo)
        {
            if (custInfo.Items.Count > 0)
            {


                var header = Request.Headers["Authorization"];
                string cusId = "";
                Customer c;
                if (header.Count > 0)
                {
                    var token = header[0].Split(" ")[1];
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    cusId = jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
                    c = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId.Equals(cusId));
                }
                else
                {
                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    bool existed = true;
                    while (existed)
                    {
                        cusId = new string(Enumerable.Repeat(chars, 5)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                        Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId.Equals(cusId));
                        if (customer == null)
                        {
                            existed = false;
                        }
                    }
                    c = new Customer
                    {
                        CustomerId = cusId,
                        CompanyName = custInfo.CompanyName != null ? custInfo.CompanyName : "",
                        ContactName = custInfo.ContactName,
                        ContactTitle = custInfo.ContactTitle,
                        Address = custInfo.Address
                    };
                    _context.Customers.Add(c);
                    _context.SaveChanges();

                }
                Order order = new Order
                {
                    OrderDate = DateTime.Now,
                    RequiredDate = custInfo.RequiredDate,
                    CustomerId = c.CustomerId,
                    ShipAddress = c.Address
                };
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                foreach (var item in custInfo.Items)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductID,
                        Quantity = (short)item.Quantity,
                        Discount = 0,
                        UnitPrice = item.UnitPrice != 0 ? item.UnitPrice.Value : 0
                    };
                    await _context.OrderDetails.AddAsync(orderDetail);
                    await _context.SaveChangesAsync();
                }
                return Ok(order);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
