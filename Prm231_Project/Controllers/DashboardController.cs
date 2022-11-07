using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Prm231_Project.DTO;
using Prm231_Project.Models;

namespace Prm231_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public DashboardController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }
        [HttpGet("[action]")]
        public IActionResult GetDashboard()
        {
            decimal totalOrders = _context.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * ((decimal)(1 - od.Discount))); 
            var totalCustomer = _context.Customers.Select(c => c.CustomerId).ToList().Count;
            var totalGuest = _context.Customers.Select(c => c.CustomerId).ToList()
                .Except(_context.Accounts.Where(a => a.CustomerId != null).Select(a => a.CustomerId).ToList())
                .ToList().Count ;
            DashboardDTO dashboardDTO = new DashboardDTO
            {
                TotalOrders = totalOrders,
                TotalCustomer = totalCustomer,
                TotalGuest = totalGuest
            };
            return Ok(dashboardDTO) ;
        }
    }
}
