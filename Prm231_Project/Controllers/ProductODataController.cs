using Prm231_Project.Models ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Prm231_Project.DTO;

namespace Prm231_Project.Controllers
{
    public class ProductODataController : ControllerBase
    {
        public PRN231DBContext dbcontext;
        public IMapper mapper;
        public ProductODataController(PRN231DBContext dbcontext)
        {
            this.dbcontext = dbcontext;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [EnableQuery(PageSize = 4)]
        public async Task<IActionResult> Get()
        {
            var products = await dbcontext.Products.Include(p => p.OrderDetails).ToListAsync();
            return Ok( products);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = dbcontext.Products.Where(p => p.ProductId == key).FirstOrDefault();
            return Ok(product);
        }

        [EnableQuery]
        public IActionResult Post([FromBody] Product product)
        {
            dbcontext.Products.Add(product);
            dbcontext.SaveChanges();
            return Created("", product);
        }

        [EnableQuery]
        public IActionResult Delete( int key)
        {
            var product = dbcontext.Products.FirstOrDefault(p => p.ProductId == key);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                dbcontext.Remove(product);
                dbcontext.SaveChanges();
                return Ok();
            }
        }
        [EnableQuery]
        public IActionResult Patch([FromBody] Product product)
        {
            var pro = dbcontext.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == product.ProductId);
            if(pro == null)
            {
                return NotFound();
            }else
            {
                dbcontext.Update<Product>(product);
                dbcontext.SaveChanges();
                return Ok(product);
            }
        }
    }
}
