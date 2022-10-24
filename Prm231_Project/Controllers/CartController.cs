using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Prm231_Project.DTO;
using Prm231_Project.Models;
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
            }else
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
        public IActionResult UpdateCartItemQuantity(int id,int quantity)
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
            }else
            {
                cart.Remove(item);
                SaveCartCookie(cart);
            }
            return Ok("Update Success!");
        }

        //[HttpPost("[action]")]
        //public IActionResult OrderCart()
        //{

        //}


    }
}
