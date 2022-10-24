using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prm231_Project.DTO;
using Prm231_Project.Models;

namespace Prm231_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public CategoryController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Category>> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories.Count() != 0)
            {
                return Ok(mapper.Map<List<CategoryDTO>>(categories));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
