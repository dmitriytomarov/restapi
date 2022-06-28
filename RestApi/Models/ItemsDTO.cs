namespace RestApi.Models
{
    /// <summary>
    /// Перечень товаров (Id + количество для создания заказа)
    /// </summary>
    public class ItemsDTO
    {
        public int Id { get; set; }
        public int Amount { get; set; }
    }
}
