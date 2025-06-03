namespace ShawarmaShopCore.Models
{
    public class Shawarma
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Ingredients { get; set; } = null!;
        public decimal Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}