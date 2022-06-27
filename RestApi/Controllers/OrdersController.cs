using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace RestApi.Controllers
{


    /*


3)	Метод получения списка заказов по конкретному клиенту за выбранный временной период, отсортированный по дате создания.

    get
orders / clientId=          сортировка по дате
orders / client id= date from= date to=          сортировка по дате

4)	Метод формирования заказа с проверкой наличия требуемого количества товара на складе, а также уменьшение доступного количества товара на складе в БД в случае успешного создания заказа. 
post
orders  товар/количество товар / количство товар количество
возврат ок частично или нет
 * 
 * */


    [Route("[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {

        private readonly ShopContext _context;
        public OrdersController(ShopContext context) { _context = context; }

        /// <summary>
        /// Получение списка всех заказов по клиенту (по Id клиента)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int id, DateTime? dateFrom=null, DateTime? dateTo=null)
        {
            if (_context.Orders == null) return NotFound("Таблица Orders не найдена");
            
            var orders = await _context.Orders.Where(e => e.Id == id).OrderBy(e => e.OrderDate).ToListAsync();

            if (dateFrom == null && dateTo == null) return orders;

            if (dateTo == null) dateTo = DateTime.Now;
            if (dateFrom == null) dateFrom = DateTime.MinValue;

            return orders.Where(e => e.OrderDate >= dateFrom).ToList();

        }





    }
}
