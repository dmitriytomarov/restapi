using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class Good
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
