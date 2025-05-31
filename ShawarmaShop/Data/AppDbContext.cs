using Microsoft.EntityFrameworkCore;
using Shawarma_.Models;
using Client_.Models;
using Order_.Models;
using OrderItem_.Models;
using Authentication;

public class AppDbContext : DbContext
{
    public DbSet<Shawarma> Shawarmas { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=shawarma.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Client)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.ClientID);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Shawarma)
            .WithMany(s => s.OrderItems)
            .HasForeignKey(oi => oi.ShawarmaId);

        modelBuilder.Entity<Shawarma>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, Name = "John Doe", Phone = "+48123456789", Email = "john@example.com" },
            new Client { Id = 2, Name = "Jane Smith", Phone = "+48987654321", Email = "jane@example.com" },
            new Client { Id = 3, Name = "Mike Johnson", Phone = "+48555666777", Email = "mike@example.com" },
            new Client { Id = 4, Name = "Anna Kowalski", Phone = "+48111222333", Email = "anna@example.com" }
        );

        modelBuilder.Entity<Shawarma>().HasData(
            new Shawarma { Id = 1, Name = "Classic Chicken", Ingredients = "Chicken, lettuce, tomato, cucumber, sauce", Price = 15.50m },
            new Shawarma { Id = 2, Name = "Beef Deluxe", Ingredients = "Beef, onion, pepper, cheese, spicy sauce", Price = 18.00m },
            new Shawarma { Id = 3, Name = "Vegetarian", Ingredients = "Falafel, lettuce, tomato, cucumber, hummus", Price = 12.50m },
            new Shawarma { Id = 4, Name = "Spicy Lamb", Ingredients = "Lamb, hot pepper, onion, garlic sauce", Price = 20.00m },
            new Shawarma { Id = 5, Name = "Mixed Meat", Ingredients = "Chicken, beef, vegetables, mixed sauce", Price = 22.00m }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, DateTime = new DateTime(2024, 05, 28, 12, 0, 0), Comment = "Extra sauce please", ClientID = 1 },
            new Order { Id = 2, DateTime = new DateTime(2024, 05, 29, 14, 30, 0), Comment = "No onions", ClientID = 2 },
            new Order { Id = 3, DateTime = new DateTime(2024, 05, 30, 16, 45, 0), Comment = null, ClientID = 3 }
        );

        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { Id = 1, OrderId = 1, ShawarmaId = 1, Quantity = 2 },
            new OrderItem { Id = 2, OrderId = 1, ShawarmaId = 3, Quantity = 1 },
            new OrderItem { Id = 3, OrderId = 2, ShawarmaId = 2, Quantity = 1 },
            new OrderItem { Id = 4, OrderId = 3, ShawarmaId = 4, Quantity = 3 },
            new OrderItem { Id = 5, OrderId = 3, ShawarmaId = 5, Quantity = 1 }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            { Id = 1, Username = "admin", PasswordHash = "21232f297a57a5a743894a0e4a801fc3", Role = "admin" }
        );
    }
}
