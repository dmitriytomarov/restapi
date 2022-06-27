using Microsoft.EntityFrameworkCore;

namespace RestApi.Models
{
    public class ShopContext : DbContext

    {

        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { Database.EnsureCreated(); }
       
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Good> Goods { get; set; } = null!;
        public DbSet<OrderBom> OrderBoms { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

    }
}
