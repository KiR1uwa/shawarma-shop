public class Order
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string? Comment { get; set; }
    public int ClientID { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}