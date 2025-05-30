using Shawarma_.Models;
using Order_.Models;

namespace OrderItem_.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ShawarmaId { get; set; }
    public Shawarma Shawarma { get; set; } = null!;
    public int Quantity { get; set; }
}