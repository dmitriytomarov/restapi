using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RestApi.Models;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ShopContext _context;
        private readonly IMemoryCache _cash;
        public CategoryController(ShopContext context, IMemoryCache cache)
        {
            _context = context;
            _cash = cache;
        }



        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories() 
        {
            List<Category> categories = new();
            if (!_cash.TryGetValue("categories", out categories))
            {
                categories= await _context.Categories.ToListAsync();
                _cash.Set("categories", categories, new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
            }

            return categories;
        }
    }
}
