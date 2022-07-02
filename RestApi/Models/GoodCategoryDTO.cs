namespace RestApi.Models
{
    public class GoodCategoryDTO
    {
        public int Id { get; set; }
        public string GoodName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // отдельный DTO класс-прослойка для возможности поменять набор выводимых клиенту данных (не как в БД)
        // например не хотим показывать клиенту ряд полей из БД, например служебных
    }
}
