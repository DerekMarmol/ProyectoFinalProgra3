using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IReadOnlyList<CartItem>> GetCartItemsByCartIdAsync(string cartId);
        Task ClearCartAsync(string cartId);
    }
}