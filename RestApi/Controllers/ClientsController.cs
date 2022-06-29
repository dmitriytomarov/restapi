using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClientsController : Controller
    {
        private readonly ShopContext _context;
        public ClientsController(ShopContext context) { _context = context; }


        /// <summary>
        /// Получение спиcка всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Client>> GetAllClients()
        {
            return   await _context.Clients.ToListAsync();
        }

        /// <summary>
        /// Получение клиента по Id  (int)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Client>> GetClient(int id)  
        {
            if (_context.Clients == null)  return NotFound($"Клиент с Id '{id}' не найден."); 

            var client = await _context.Clients.FindAsync(id);
            if (client == null) { return NotFound($"Клиент с Id '{id}' не найден."); }

            return client;
        }

       
    }
}
        