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

    [ApiController]
    [Route("[controller]")]
    public class GoodsController : Controller
    {
        private readonly ShopContext _context;
        public GoodsController(ShopContext context) { _context = context; }

        /// <summary>
        /// Получение списка всех товаров
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<Good>> GetGoods()
        {
            return await _context.Goods.ToListAsync();
        }


        /// <summary>
        /// Получение списка товаров, с возможностью фильтрации по типу товара, по наличию на складе и сортировки по цене
        /// </summary>
        /// <remarks>
        /// Строка запроса может содержать параметры: "category", "minstock", "sort".
        /// category - имя категории (string).
        /// minstock - требуемое количество на складе (int).
        /// sort - сортировка (string). Возможные значения:  price (сортировка по возрастанию цены), pricedesc (сортировка по убыванию цены)
        /// </remarks>
        /// <param name="querystring">
        /// Примеры строк  
        /// <![CDATA[ ?category=Cat1&minstock=10&sort=pricedesc]]><br/>
        /// <![CDATA[ ?sort=price]]><br/> 
        /// <![CDATA[ ?minstock=1&category=Cat3]]>
        /// </param>
        /// <returns></returns>
        [HttpGet("filter/{querystring}")]
        public async Task<ActionResult<IEnumerable<Good>>> GetGoodsBy(string? querystring = "")
        {

            List<string> valids = new() { "category", "minstock", "sort" }; // список допустимых параметров
            Dictionary<string, string> parameters = new();

            if (!String.IsNullOrEmpty(querystring))
            {
                querystring = querystring.Replace(@"?", "");
                string[] q = querystring.Split("&");

                foreach (var item in q)
                {
                    string key = item.Split('=')[0];
                    string value = item.Split('=')[1];
                    if (valids.Contains(key)) // если указан параметр кроме валидных - просто игнорируем. можно сделать тоже BadRequest (??)
                    {
                        if (!parameters.ContainsKey(key)) { parameters.Add(key, value); }
                        else 
                        {
                            return BadRequest($"Ошибка: параметр '{key}' указан дважды");
                        } 
                    }
                }
            }

            string sort = "";
            StringBuilder where = new();
            List <string> whereConditions = new();
            
            if (parameters.ContainsKey("category"))
            {
                whereConditions.Add ($@"Categories.CategoryName = '{parameters["category"]}'");
            }
            
            
            if (parameters.ContainsKey("minstock"))  //не понял как нужно, просто факт наличия на складе, или по запрошенному количеству? сделал второе
            {
                if (!int.TryParse(parameters["minstock"], out _)) return BadRequest("Количество на складе: неверный формат");
                whereConditions.Add($@"Goods.Stock >= {parameters["minstock"]}");
            }


            where.Append( whereConditions.Count>0 ? "WHERE " : "").Append(String.Join(" And ", whereConditions));
            
            if (parameters.ContainsKey("sort"))
            {
                switch (parameters["sort"])
                {
                    case "price":
                        sort = " ORDER BY Goods.Price"; break;
                    case "pricedesc":
                        sort = " ORDER BY Goods.Price DESC"; break;
                    default:
                        return BadRequest($"Ошибка: значение sort '{parameters["sort"]}' не поддердживается");
                }
            }

            string request = @$"SELECT Goods.Id,
                                         Goods.Category,
                                         Goods.Price,
                                         Goods.Stock,
                                         Categories.CategoryName,
                                         Goods.Name
                                    FROM Goods 
                                    JOIN Categories on Goods.Category = Categories.Id {where} {sort}";


            return await _context.Goods.FromSqlRaw(request).ToListAsync();
        }


        /// <summary>
        /// Вариант 2 (Отдельными полями). Получение списка товаров, с возможностью фильтрации по типу товара, по наличию на складе и сортировки по цене
        /// </summary>
        /// <remarks>
        /// Строка запроса может содержать параметры: "category", "minstock", "sort".
        /// category - имя категории (string).
        /// minstock - требуемое количество на складе (int).
        /// sort - сортировка (string). Возможные значения:  price (сортировка по возрастанию цены), pricedesc (сортировка по убыванию цены)
        /// </remarks>
        /// <param>
        /// Параметры  <br/> 
        /// category  (Cat1, Cat2, Cat3, Cat4). Название категории <br/> 
        /// minstock  (int).  Требуемое к наличию на складе количество 
        /// sort     Возможные знначения: price (по возрастанию цены),  pricedesc (по убыванию цены)
        /// </param>
        /// <returns></returns>
        [HttpGet("filterVar2/")]
        public async Task<ActionResult<IEnumerable<Good>>> GetGoodsByVar2(string? category =null, string? minstock=null, string? sort=null)
        {

            StringBuilder where = new();
            List<string> whereConditions = new();

            if (!String.IsNullOrEmpty(category))
            {
                whereConditions.Add($@"Categories.CategoryName = '{category}'");
            }

            if (!String.IsNullOrEmpty(minstock))  
            {
                if (!int.TryParse(minstock, out _)) return BadRequest("Количество на складе: неверный формат");
                whereConditions.Add($@"Goods.Stock >= {minstock}");
            }

            where.Append(whereConditions.Count > 0 ? "WHERE " : "").Append(String.Join(" And ", whereConditions));

            if (!String.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "price":
                        sort = " ORDER BY Goods.Price"; break;
                    case "pricedesc":
                        sort = " ORDER BY Goods.Price DESC"; break;
                    default:
                        return BadRequest($"Ошибка: значение sort '{sort}' не поддердживается");
                }
            }

            string request = @$"SELECT Goods.Id,
                                         Goods.Category,
                                         Goods.Price,
                                         Goods.Stock,
                                         Categories.CategoryName,
                                         Goods.Name
                                    FROM Goods 
                                    JOIN Categories on Goods.Category = Categories.Id {where} {sort}";


            return await _context.Goods.FromSqlRaw(request).ToListAsync();
        }

    }
}
