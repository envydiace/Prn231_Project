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
        public async Task< ActionResult<List<Customer>>> GetAllCustomer()
        {
            var result = await _context.Customers.ToListAsync();
            return Ok(result);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task< ActionResult<Customer>> GetCustomer()
        {
            var CustomerId = GetCustomerID();
            var customer = await _context.Customers.Include(c => c.Accounts).Where(c => c.CustomerId.Equals(CustomerId)).FirstOrDefaultAsync();
            return Ok(mapper.Map<CustomerDTO>(customer));
        }
        [Authorize]
        [HttpPut("[action]")]
        public async Task<IActionResult> EditCustomerInfo([FromForm] CustomerEditDTO customerDTO)
        {
            var CustomerId = GetCustomerID();
            var cus = await _context.Customers.Where(c => c.CustomerId.Equals(CustomerId)).AsNoTracking().FirstOrDefaultAsync();
            if (cus != null)
            {
                if (ModelState.IsValid)
                {
                    cus.ContactName = customerDTO.ContactName;
                    cus.CompanyName = customerDTO.CompanyName;
                    cus.ContactTitle = customerDTO.ContactTitle;
                    cus.Address = customerDTO.Address;
                    _context.Update<Customer>(cus);
                    Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.CustomerId.Equals(CustomerId));
                    account.Email = customerDTO.Email;
                    _context.Update<Account>(account);
                    _context.SaveChanges();
                    return Ok(cus);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return NotFound();
            }

        }
        private string GetCustomerID()
        {
            var header = Request.Headers["Authorization"];
            var token = header[0].Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
        }
    }
}
