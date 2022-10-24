using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prm231_Project.DTO;
using Prm231_Project.Models;

namespace Prm231_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public ProductController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [HttpGet("[action]")]
        public ActionResult<List<ProductDTO>> GetAllProduct()
        {
            return Ok(mapper.Map<List<ProductDTO>>(_context.Products.Include(p => p.Category)));
        }
        [HttpGet("[action]")]
        public IActionResult GetProductHot(int categoryId, int pageIndex = 1, int pageSize = 4)
        {
            var list = (from orderDetail in _context.OrderDetails 
                       group orderDetail by orderDetail.ProductId into product
                       select new
                       {
                           ProductId = product.Key,
                           Discount = product.Max(p => p.Discount)
                       }).OrderByDescending(od => od.Discount)
                       .ThenByDescending(od => od.ProductId)
                       .Join(_context.Products, od => od.ProductId, p => p.ProductId, 
                       (od ,p) => p)
                       .Where( p => categoryId == 0 || p.CategoryId == categoryId)
                       ;
            int total = list.Count();
            int totalPages = total % pageSize == 0 ? (total / pageSize) : ((total / pageSize) + 1);
            var value = mapper.Map<ICollection<ProductDTO>>(
                list.Include(p => p.Category)
                .Include(p => p.OrderDetails)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize));
            return Ok(new PagingProductDTO
            {
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                Values = value
            });
        }
        [HttpGet("[action]")]
        public ActionResult<PagingProductDTO> GetProductBestSale(int categoryId= 0, int pageIndex = 1, int pageSize = 4)
        {
            var list = _context.Products.Where(  p => categoryId == 0 || p.CategoryId == categoryId);
            int total = list.Count();
            int totalPages = total % pageSize == 0 ? (total / pageSize) : ((total / pageSize) + 1);
            var value = mapper.Map<ICollection<ProductDTO>>(
                list.Include(p => p.Category)
                .Include(p => p.OrderDetails)
                .OrderByDescending(p => p.OrderDetails.Count)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize));
            return Ok(new PagingProductDTO
            {
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                Values = value
            });
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _context.Products.Include(p => p.Category).Where(p => p.ProductId == id).FirstOrDefaultAsync();
            if (product != null)
            {
                return Ok(mapper.Map<ProductDTO>(product));
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromForm]ProductAddDTO product)
        {
            if (ModelState.IsValid)
            {
                var p = mapper.Map<Product>(product);
                await _context.AddAsync<Product>(p);
                _context.SaveChanges();
                return CreatedAtAction("Get", new { id = (p != null ? p.ProductId : 0) }, p);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("[action]/{id}")]
        public ActionResult<string> Delete(int id)
        {
            try
            {
                var product = _context.Products
                .Where(e => e.ProductId == id).FirstOrDefault();
                if (product != null)
                {
                    _context.Remove<Product>(product);
                    _context.SaveChanges();
                    return Ok("Delete success product with id = " + id + "!");
                }
                else
                {
                    return BadRequest("Product id " + id + " doesn't existed!");
                }
            }
            catch (Exception)
            {
                return BadRequest("Can not delete product with id = " + id + "!");
            }

        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("[action]")]
        public IActionResult Update([FromForm]ProductEditDTO product)
        {
            var emp = _context.Products.Where(e => e.ProductId == product.ProductId).AsNoTracking().FirstOrDefault();
            if (emp != null)
            {
                if (ModelState.IsValid)
                {
                    var p = mapper.Map<Product>(product);
                    _context.Update<Product>(p);
                    _context.SaveChanges();
                    return Ok(p);
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

        [Authorize(Policy = "AdminOnly")]
        //[Authorize(Roles = "1")]
        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDTO>> GetAllFilter(int categoryId, string? search = null)
        {
            if (search == null)
            {
                search = "";
            }
            var products = await _context.Products.Include(p => p.Category)
                .OrderByDescending(p => p.ProductId)
                .Where(p =>
                (categoryId == 0 || p.CategoryId == categoryId)
                && p.ProductName.Contains(search))
                .ToListAsync();

            if (products.Count() != 0)
            {
                return Ok(mapper.Map<List<ProductDTO>>(products));
            }
            else
            {
                return Ok(new List<ProductDTO>());
            }
        }

    }
}
