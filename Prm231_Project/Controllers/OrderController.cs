using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prm231_Project.DTO;
using Prm231_Project.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Prm231_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public OrderController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }
        [HttpGet("[action]")]
        [Authorize(Policy = "CustOnly")]
        public async Task<IActionResult> GetCustomerOrder()
        {
            var header = Request.Headers["Authorization"];
            var token = header[0].Split(" ")[1];

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var CustomerId = jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
            var result = await _context.Orders.Where(o => o.CustomerId.Equals(CustomerId) && o.RequiredDate != null)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
            return Ok(mapper.Map<List<OrderDTO>>(result));
        }

        [HttpGet("[action]")]
        [Authorize(Policy = "CustOnly")]
        public async Task<IActionResult> GetCustomerCanceledOrder()
        {
            var header = Request.Headers["Authorization"];
            var token = header[0].Split(" ")[1];

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var CustomerId = jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
            var result = await _context.Orders.Where(o => o.CustomerId.Equals(CustomerId) && o.RequiredDate == null)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
            return Ok(mapper.Map<List<OrderDTO>>(result));
        }

        [HttpGet("[action]")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllOrders(DateTime? from = null, DateTime? to = null, int pageIndex = 1, int pageSize = 8)
        {
            if ((from != null && DateTime.MaxValue < from) || (from != null && new DateTime(1753, 1, 1) > from)
                || (to != null && DateTime.MaxValue < to) || (to != null && new DateTime(1753, 1, 1) > to))
            {
                return BadRequest("Invalid DateTime");
            }

            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Where(o =>
                (from == null || o.OrderDate >= from) &&
                (to == null || o.OrderDate <= to)
                )
                .ToListAsync();

            int total = orders.Count();
            int totalPages = total % pageSize == 0 ? (total / pageSize) : ((total / pageSize) + 1);
            var value = mapper.Map<List<OrderAdminDTO>>(
                orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                );

            return Ok(new PagingOrderDTO
            {
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                Values = value
            });
        }

        [HttpPut("[action]")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return BadRequest("Order doesn't existed!");
                }
                else
                {
                    if (order.ShippedDate == null)
                    {
                        order.RequiredDate = null;
                        _context.Update<Order>(order);
                        await _context.SaveChangesAsync();
                        return Ok(mapper.Map<OrderDTO>(order));
                    }
                    else
                    {
                        return NotFound("Can not cancel this order!");
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest("Somethings happend!");
            }

        }
    }
}
