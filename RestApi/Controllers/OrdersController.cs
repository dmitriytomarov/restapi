using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    public class OrdersController : ControllerBase
    {
    }
}
