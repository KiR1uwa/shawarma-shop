public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}