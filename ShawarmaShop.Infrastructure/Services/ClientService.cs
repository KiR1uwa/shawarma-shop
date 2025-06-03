// -----------------------------------------------------------------------------
// ClientService.cs
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShawarmaShop.Infrastructure.Data;
using ShawarmaShopCore.Interfaces;
using ShawarmaShopCore.Models;

namespace ShawarmaShopCore.Services
{
    /// <summary>
    ///     Concrete implementation of <see cref="IClientService"/> that performs
    ///     CRUD operations for <see cref="Client"/> entities using an
    ///     <see cref="AppDbContext"/>. No business logic is applied beyond basic
    ///     persistence; validation is expected to be handled by the caller or by
    ///     separate validators.
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        /// <summary>
        ///     Creates a new <see cref="ClientService"/> bound to the supplied
        ///     <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An <see cref="AppDbContext"/> instance.</param>
        public ClientService(AppDbContext context) => _context = context;

        /// <inheritdoc/>
        public IEnumerable<Client> GetAllClients() => _context.Clients.ToList();

        /// <inheritdoc/>
        public Client? GetClientById(int id) => _context.Clients.FirstOrDefault(c => c.Id == id);

        /// <inheritdoc/>
        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        /// <inheritdoc/>
        public void UpdateClient(Client client)
        {
            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        /// <inheritdoc/>
        public void DeleteClient(int id)
        {
            var client = GetClientById(id);
            if (client is not null)
            {
                _context.Clients.Remove(client);
                _context.SaveChanges();
            }
        }
    }
}