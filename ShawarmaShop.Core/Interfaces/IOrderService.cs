using System.Collections.Generic;
using ShawarmaShopCore.Models;

namespace ShawarmaShopCore.Interfaces
{
    public interface IOrderService
    {
        decimal RecalculateTotal(List<OrderItem> items);
        void PlaceOrder(Order order);
    }
}
