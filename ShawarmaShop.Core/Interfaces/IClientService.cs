using System.Collections.Generic;
using ShawarmaShopCore.Models;

namespace ShawarmaShopCore.Interfaces
{
    public interface IClientService
    {
        IEnumerable<Client> GetAllClients();
        Client GetClientById(int id);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int id);
    }
}
