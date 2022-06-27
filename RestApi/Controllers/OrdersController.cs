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
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int id)
        {

            return await _context.Orders.Where(e => e.Id == id).OrderBy(e=> e.OrderDate).ToListAsync();

        }

        [HttpGet("/bydate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<Order>> GetOrdersByDate(DateTime datefrom, string? date2=null )
        {
            Debug.WriteLine("AAAAAAAAAAA");
            Debug.WriteLine(datefrom);
            return await _context.Orders.ToListAsync();
        }


        [HttpGet("{firstName}/{lastName}/{address}")]
        public string GetQuery(string id, string firstName, string lastName, string address)
        {
            return $"{firstName}:{lastName}:{address}";
        }





    }
}
