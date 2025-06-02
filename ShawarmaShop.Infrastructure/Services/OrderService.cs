using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShawarmaShop.Infrastructure.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync() =>
            await _context.Orders.Include(o => o.Items).ToListAsync();

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders.Include(o => o.Items)
                                 .FirstOrDefaultAsync(o => o.Id == id);

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

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
