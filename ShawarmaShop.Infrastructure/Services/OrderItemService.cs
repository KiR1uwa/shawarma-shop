using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShawarmaShop.Infrastructure.Services
{
    public class OrderItemService
    {
        private readonly AppDbContext _context;

        public OrderItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderItem>> GetAllAsync() =>
            await _context.OrderItems.Include(i => i.Shawarma).ToListAsync();

        public async Task AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

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
