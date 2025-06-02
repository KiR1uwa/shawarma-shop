using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShawarmaShop.Infrastructure.Services
{
    public class ShawarmaService
    {
        private readonly AppDbContext _context;

        public ShawarmaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Shawarma>> GetAllAsync() =>
            await _context.Shawarmas.ToListAsync();

        public async Task<Shawarma?> GetByIdAsync(int id) =>
            await _context.Shawarmas.FindAsync(id);

        public async Task AddAsync(Shawarma shawarma)
        {
            _context.Shawarmas.Add(shawarma);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shawarma shawarma)
        {
            _context.Shawarmas.Update(shawarma);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Shawarmas.FindAsync(id);
            if (item is not null)
            {
                _context.Shawarmas.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
