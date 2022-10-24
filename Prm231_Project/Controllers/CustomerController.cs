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
    public class CustomerController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public CustomerController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<List<Customer>> GetAllCustomer()
        {
            return Ok(_context.Customers);
        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<Customer> GetCustomer()
        {
            var header = Request.Headers["Authorization"];
            var token = header[0].Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var CustomerId = jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
            var customer = _context.Customers.Include(c => c.Accounts).Where(c => c.CustomerId.Equals(CustomerId)).FirstOrDefault();
            return Ok(mapper.Map<CustomerDTO>(customer));
        }
    }
}
