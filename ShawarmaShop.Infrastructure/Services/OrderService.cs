// -----------------------------------------------------------------------------
// OrderService.cs
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using ShawarmaShop.Infrastructure.Data;
using ShawarmaShopCore.Models;

namespace ShawarmaShop.Infrastructure.Services
{
    /// <summary>
    ///     Provides basic asynchronous CRUD operations for <see cref="Order"/>
    ///     entities. Consumers should layer additional validation and business
    ///     rules on top of this service as needed.
    /// </summary>
    public class OrderService
    {
        private readonly AppDbContext _context;

        /// <summary>Initialises the service with the supplied context.</summary>
        public OrderService(AppDbContext context) => _context = context;

        /// <summary>
        ///     Returns all orders, eager‑loading their <c>Items</c> collection to
        ///     avoid the N+1 query problem when enumerating in the caller.
        /// </summary>
        public async Task<List<Order>> GetAllAsync() =>
            await _context.Orders.Include(o => o.Items).ToListAsync();

        /// <summary>
        ///     Retrieves a single order by id, including its items.
        /// </summary>
        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders.Include(o => o.Items)
                                 .FirstOrDefaultAsync(o => o.Id == id);

        /// <summary>
        ///     Persists a new order.
        /// </summary>
        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Deletes an order by id, if it exists.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is not null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
