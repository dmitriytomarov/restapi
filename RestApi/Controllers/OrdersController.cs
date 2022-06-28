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



4)	Метод формирования заказа с проверкой наличия требуемого количества товара на складе, а также уменьшение доступного количества товара
    на складе в БД в случае успешного создания заказа. 
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
        /// Получение списка всех заказов по клиенту с возможностью фильтрации по дате
        /// </summary>
        /// <remarks>
        /// Параметры <br/>
        /// id клиента  (int).<br/>
        /// dateFrom (опционально) - дата в формате dd.mm.YYYY или YYYY.mm.dd<br/>
        /// dateTo (опционально) - дата в формате dd.mm.YYYY или YYYY.mm.dd
        /// </remarks>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int id, DateTime? dateFrom=null, DateTime? dateTo=null)
        {
            if (_context.Orders == null) return NotFound("Таблица Orders не найдена");
            
            var orders = await _context.Orders.Where(e => e.Client == id).OrderBy(e => e.OrderDate).ToListAsync();

            if (dateFrom == null && dateTo == null) return orders;

            if (dateTo == null) dateTo = DateTime.Now;
            if (dateFrom == null) dateFrom = DateTime.MinValue;

            return orders.Where(e=> (e.OrderDate >= dateFrom && e.OrderDate <= dateTo)).ToList();
        }

        /// <summary>
        /// Создание нового заказа
        /// </summary>
        /// <remarks>Входные данные: перечень в формате JSON:  id товара, количество товара</remarks>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("new")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<OrderItem>>> CreateOrder(List<ItemsDTO> bom)
        {
            Dictionary<int, int> requested = new(); //то что запрошено клиентом (поля id и количество)
            List<Good> requestedGoods = new(); // запрошенные товары, выборка из БД (все поля)
            List<OrderItem> order = new();
            bool flag = true;

            foreach (var item in bom)
            {
                try
                {
                    if (!requested.ContainsKey(item.Id))
                    {
                        requested.Add(item.Id, item.Amount);
                    }
                    else
                    {
                        requested[item.Id] += item.Amount;
                    }

                    if (item.Id <= 0) return BadRequest($"Ошибка: некорректное значение Id:  '{item.Id}'");
                }
                catch (Exception)
                {
                    return BadRequest("Ошибка: некорректное значение Id или количества");
                }
            }
            if (!requested.Any()) return BadRequest("Пустой запрос");

            string set = string.Join(", ", requested.Keys);
            string request = @$"SELECT * FROM Goods WHERE Goods.Id IN ({set})";

            requestedGoods = await _context.Goods.FromSqlRaw(request).ToListAsync();  //выборка из БД запрошенных товаров

            Good good;
            OrderItem line;
            int position = 1;
            int orderMaxId = await _context.Orders.Select(e => e.Id).MaxAsync();

            foreach (var i in requested)
            {
                good = requestedGoods.FirstOrDefault(e => e.Id == i.Key)!;

                if (i.Value <= good?.Stock)
                {
                    if (i.Value < 0) return BadRequest($"Ошибка:  Отрицательное количество для товара с Id '{i.Key}'");
                    if (i.Value == 0) continue;
                    line = new OrderItem  // для вывода потом в итог. можно убрать если вывод не нужен.
                    {
                        Position = position++,
                        OrderId = orderMaxId + 1,
                        GoodId = good.Id,
                        Price = good.Price,
                        Amount = i.Value
                    };
                    
                    _context.OrderItems.Add(line);
                    order.Add(line); // для вывода в JSON резалт. если не надо то убрать

                    Debug.WriteLine($"добавлен bl {good.Id}  цена {good.Price} количество {good.Stock}");
                    good.Stock -= i.Value;
                    Debug.WriteLine($"количество  {good.Id}  уменьшено на  {i.Value} новое колво  {good.Stock}");
                }
                else
                {
                    Debug.WriteLine($"количество недостаточно товар ид {good.Id}  складк {good.Stock}");
                    return BadRequest($"Отклонено:  Недостаточное количество на складе для Id '{good.Id}'");
                }
            }
            if (!order.Any()) return BadRequest("Ошибка: пустой заказ");

            if (flag)
            {
                foreach (var item in requestedGoods)
                {
                    _context.Goods.FirstOrDefault(e => e.Id == item.Id)!.Stock = item.Stock;
                    Debug.WriteLine($"флаг    количество для {item.Id} новое = {item.Stock}");
                }
                _context.SaveChanges();
            }

            return order;
        }
    }
}
