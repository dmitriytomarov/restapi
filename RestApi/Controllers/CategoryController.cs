using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly ShopContext _context;

        public CategoryController(ShopContext context)
        {
            _context = context;
        }



        //[HttpGet("categ")]
        //public async Task<IActionResult> Details(int? id)
        //public async Task<ActionResult<IEnumerable<Client>>> Details(int? id)  //работает11
        public async Task<ActionResult<Category>> Cat(int? id)  //работает22
        {



            return await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);// работает22

            

        }
    }
}
