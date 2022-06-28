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
        //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<ActionResult<IEnumerable<OrderItem>>> CreateOrder(List<ItemsDTO> bom)
        //public async Task<IActionResult<IEnumerable<OrderItem>>> CreateOrder(List<ItemsDTO> bom)
        {
            List<int> requestedIds = new();
            List<int> requestedQty = new();
            List<Good> requestedGoods = new();
            List<OrderItem> order = new();
            bool flag = true;

            foreach (var item in bom)
            {
                try
                {
                    requestedIds.Add(item.Id);
                    requestedQty.Add(item.Amount);
                    if (item.Id <= 0) return BadRequest($"Ошибка: некорректное значение Id:  '{item.Id}'");
                }
                catch (Exception)
                {
                    //requestedIds.Clear();
                    //requestedQty.Clear();
                    return BadRequest("Ошибка: некорректное значение Id или количества");
                }
            }
            if (!requestedIds.Any()) return BadRequest("Пустой запрос");

            string set = string.Join(", ", requestedIds);
            string request = @$"SELECT * FROM Goods WHERE Goods.Id IN ({set})";

            requestedGoods = await _context.Goods.FromSqlRaw(request).ToListAsync();
            //выборка из БД всех товаров которые запрошены

            
            Good good;
            OrderItem line;
            int position = 1;
            for (int i = 0; i < requestedIds.Count; i++)
            {
                good = requestedGoods.FirstOrDefault(e => e.Id == requestedIds[i])!;
                if (requestedQty[i] <= good?.Stock)
                {

                    line = new OrderItem
                    {
                        Position = position++,
                        OrderId = 1,
                        GoodId = good.Id,
                        Price = good.Price,
                        Amount = requestedQty[i]
                    };
                    //_context.OrderItems.Add(new OrderItem   // если итоговый заказ не надо в вывод то оставить так 
                    //{
                    //    Position = position++,
                    //    OrderId = 1,
                    //    GoodId = good.Id,
                    //    Price = good.Price,
                    //    Amount = requestedQty[i]
                    //});

                    _context.OrderItems.Add(line);
                    order.Add(line); // для вывода в JSON резалт. если не надо то убрать

                    Debug.WriteLine($"добавлен bl {good.Id}  цена {good.Price} количество {good.Stock}");
                    good.Stock -= requestedQty[i];
                    Debug.WriteLine($"количество  {good.Id}  уменьшено на  {requestedQty[i]} новое колво  {good.Stock}");
                }
                else
                {
                    Debug.WriteLine($"количество недостаточно товар ид {good.Id}  складк {good.Stock}");
                    return BadRequest($"Отклонено:  Недостаточное количество на складе для Id'{good.Id}'");
                }
            }

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
