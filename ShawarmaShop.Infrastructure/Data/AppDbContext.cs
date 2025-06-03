// =============================================================
// File: AppDbContext.cs
// =============================================================

using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using ShawarmaShopCore.Models;
using Authentication;

namespace ShawarmaShop.Infrastructure.Data
{
    /// <summary>
    ///     Entity Framework Core database context for the **Shawarma Shop** application.
    ///     <para>
    ///         Exposes <see cref="DbSet{TEntity}"/> wrappers for every aggregate root in the domain
    ///         model and configures SQLite as the underlying provider when no external
    ///         options are supplied.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     Two constructors are provided:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see cref="AppDbContext(DbContextOptions{AppDbContext})"/> – used by ASP.NET Core
    ///                 dependency‑injection or test suites that wish to supply their own options
    ///                 (e.g. an in‑memory provider).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Parameter‑less <see cref="AppDbContext()"/> – falls back to SQLite in the
    ///                 executable’s directory (<c>shawarma.db</c>).  Handy for WPF scenarios where
    ///                 no DI container is available.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class AppDbContext : DbContext
    {
        // ---------------------------------------------------------------------
        // DbSet<T> properties
        // ---------------------------------------------------------------------

        /// <summary>Menu items available for ordering.</summary>
        public DbSet<Shawarma> Shawarmas { get; set; } = null!;

        /// <summary>Registered clients and walk‑ins.</summary>
        public DbSet<Client> Clients { get; set; } = null!;

        /// <summary>Orders placed by clients (parent in a 1‑to‑many relationship with <see cref="OrderItem"/>).</summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>Line items belonging to an <see cref="Order"/>.</summary>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        /// <summary>Application users – currently only <c>admin</c> and regular customers.</summary>
        public DbSet<User> Users { get; set; } = null!;

        // ---------------------------------------------------------------------
        // Constructors
        // ---------------------------------------------------------------------

        /// <summary>
        ///     Initializes the context with externally supplied options.  Recommended for
        ///     production usage where a DI container provides the configured <see cref="DbContextOptions"/>.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        ///     Parameter‑less constructor that defers provider selection to
        ///     <see cref="OnConfiguring"/>.  Designed mainly for desktop apps and quick tests.
        /// </summary>
        public AppDbContext() { }

        // ---------------------------------------------------------------------
        // Configuration & model building
        // ---------------------------------------------------------------------

        /// <summary>
        ///     Configures the database provider and connection string if they haven’t been
        ///     set already (e.g. by <c>optionsBuilder</c> supplied in the constructor).
        /// </summary>
        /// <param name="optionsBuilder">Fluent builder exposed by EF Core.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbPath = Path.Combine(AppContext.BaseDirectory, "shawarma.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        /// <summary>
        ///     Configures entity relationships, property mappings and seeds initial data
        ///     (demo clients, menu items, orders, etc.).
        /// </summary>
        /// <param name="modelBuilder">Fluent builder used to construct the model metadata.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------- Relations ----------------
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

            // ---------------- Property mapping ----------------
            modelBuilder.Entity<Shawarma>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            // ---------------- Seed sample data ----------------
            SeedData(modelBuilder);
        }

        // ---------------------------------------------------------------------
        // Seeding helpers
        // ---------------------------------------------------------------------

        /// <summary>
        ///     Populates the database with a small set of demo data so that a fresh clone
        ///     of the repository can run immediately without manual data entry.
        /// </summary>
        /// <param name="modelBuilder">Builder instance from <see cref="OnModelCreating"/>.</param>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            // --- Clients ------------------------------------------------------
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "John Doe", Phone = "+48123456789", Email = "john@example.com" },
                new Client { Id = 2, Name = "Jane Smith", Phone = "+48987654321", Email = "jane@example.com" },
                new Client { Id = 3, Name = "Mike Johnson", Phone = "+48555666777", Email = "mike@example.com" },
                new Client { Id = 4, Name = "Anna Kowalski", Phone = "+48111222333", Email = "anna@example.com" }
            );

            // --- Menu (Shawarmas) -------------------------------------------
            modelBuilder.Entity<Shawarma>().HasData(
                new Shawarma { Id = 1, Name = "Classic Chicken", Ingredients = "Chicken, lettuce, tomato, cucumber, sauce", Price = 15.50m },
                new Shawarma { Id = 2, Name = "Beef Deluxe", Ingredients = "Beef, onion, pepper, cheese, spicy sauce", Price = 18.00m },
                new Shawarma { Id = 3, Name = "Vegetarian", Ingredients = "Falafel, lettuce, tomato, cucumber, hummus", Price = 12.50m },
                new Shawarma { Id = 4, Name = "Spicy Lamb", Ingredients = "Lamb, hot pepper, onion, garlic sauce", Price = 20.00m },
                new Shawarma { Id = 5, Name = "Mixed Meat", Ingredients = "Chicken, beef, vegetables, mixed sauce", Price = 22.00m }
            );

            // --- Orders -------------------------------------------------------
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, CreatedAt = new DateTime(2024, 5, 28, 12, 0, 0), DeliveryAt = new DateTime(2024, 5, 28, 12, 30, 0), Comment = "Extra sauce please", ClientID = 1 },
                new Order { Id = 2, CreatedAt = new DateTime(2024, 5, 29, 14, 30, 0), DeliveryAt = new DateTime(2024, 5, 29, 15, 0, 0), Comment = "No onions", ClientID = 2 },
                new Order { Id = 3, CreatedAt = new DateTime(2024, 5, 30, 16, 45, 0), DeliveryAt = new DateTime(2024, 5, 30, 17, 15, 0), Comment = null, ClientID = 3 }
            );

            // --- Order items --------------------------------------------------
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { Id = 1, OrderId = 1, ShawarmaId = 1, Quantity = 2 },
                new OrderItem { Id = 2, OrderId = 1, ShawarmaId = 3, Quantity = 1 },
                new OrderItem { Id = 3, OrderId = 2, ShawarmaId = 2, Quantity = 1 },
                new OrderItem { Id = 4, OrderId = 3, ShawarmaId = 4, Quantity = 3 },
                new OrderItem { Id = 5, OrderId = 3, ShawarmaId = 5, Quantity = 1 }
            );

            // --- Users --------------------------------------------------------
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "21232f297a57a5a743894a0e4a801fc3", // MD5("admin")
                    Role = "admin",
                    Phone = "+48000000000",
                    Email = "admin@example.com"
                }
            );
        }
    }
}
