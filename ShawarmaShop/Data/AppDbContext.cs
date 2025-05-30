using Microsoft.EntityFrameworkCore;
using Shawarma_.Models;
using Client_.Models;
using Order_.Models;
using OrderItem_.Models;

public class AppDbContext : DbContext
{ 
    public DbSet<Shawarma> Shawarmas { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=shawarma.db");
    }
}