namespace ShawarmaShopCore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeliveryAt { get; set; }
        public string? Comment { get; set; }
        public int ClientID { get; set; }
        public Client Client { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
