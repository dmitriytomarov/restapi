using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ShopContext _context;
        public CategoryController(ShopContext context) { _context = context; }

        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories() 
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
