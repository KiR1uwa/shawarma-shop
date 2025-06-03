// -----------------------------------------------------------------------------
// OrderItemService.cs
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using ShawarmaShop.Infrastructure.Data;
using ShawarmaShopCore.Models;

namespace ShawarmaShop.Infrastructure.Services
{
    /// <summary>
    ///     Small repository‑style service that exposes asynchronous CRUD methods for
    ///     <see cref="OrderItem"/> entities. Designed primarily for unit tests and
    ///     background jobs that manipulate order items outside the WPF UI.
    /// </summary>
    public class OrderItemService
    {
        private readonly AppDbContext _context;

        /// <summary>
        ///     Constructs the service with the given EF Core <paramref name="context"/>.
        /// </summary>
        public OrderItemService(AppDbContext context) => _context = context;

        /// <summary>
        ///     Retrieves every <see cref="OrderItem"/> with its related
        ///     <see cref="Shawarma"/> in a single round‑trip.
        /// </summary>
        public async Task<List<OrderItem>> GetAllAsync() =>
            await _context.OrderItems.Include(i => i.Shawarma).ToListAsync();

        /// <summary>
        ///     Persists a new <paramref name="item"/> to the database.
        /// </summary>
        public async Task AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Deletes an <see cref="OrderItem"/> by primary key if it exists.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item is not null)
            {
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}