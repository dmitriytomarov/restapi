namespace RestApi.Models
{
    public class OrderBom
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int OrderId { get; set; }
        public int GoodId { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}
