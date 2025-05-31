using OrderItem_.Models;
using Shawarma_.Models;
using Order_.Models;
using Client_.Models;

namespace Order_.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string? Comment { get; set; }

    public int ClientID { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
