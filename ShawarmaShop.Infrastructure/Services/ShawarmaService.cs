// -----------------------------------------------------------------------------
// ShawarmaService.cs
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using ShawarmaShop.Infrastructure.Data;
using ShawarmaShopCore.Models;

namespace ShawarmaShop.Infrastructure.Services
{
    /// <summary>
    ///     Service that exposes CRUD operations for <see cref="Shawarma"/> menu items.
    ///     All methods are asynchronous and leverage Entity Framework Core.
    /// </summary>
    public class ShawarmaService
    {
        private readonly AppDbContext _context;

        /// <summary>Creates a new instance bound to the given context.</summary>
        public ShawarmaService(AppDbContext context) => _context = context;

        /// <summary>Returns the full menu.</summary>
        public async Task<List<Shawarma>> GetAllAsync() =>
            await _context.Shawarmas.ToListAsync();

        /// <summary>Finds a single <see cref="Shawarma"/> by primary key.</summary>
        public async Task<Shawarma?> GetByIdAsync(int id) =>
            await _context.Shawarmas.FindAsync(id);

        /// <summary>Adds a new menu item.</summary>
        public async Task AddAsync(Shawarma shawarma)
        {
            _context.Shawarmas.Add(shawarma);
            await _context.SaveChangesAsync();
        }

        /// <summary>Updates an existing menu item.</summary>
        public async Task UpdateAsync(Shawarma shawarma)
        {
            _context.Shawarmas.Update(shawarma);
            await _context.SaveChangesAsync();
        }

        /// <summary>Deletes a menu item by id, if found.</summary>
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