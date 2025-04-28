using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;

namespace SuperBodega.Core.Services
{
    public interface ICartService
    {
        Task<CartItemDto> AddItemToCartAsync(string cartId, int productId, int quantity);
        Task<bool> RemoveItemFromCartAsync(string cartId, int productId);
        Task<bool> UpdateCartItemQuantityAsync(string cartId, int productId, int quantity);
        Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string cartId);
        Task ClearCartAsync(string cartId);
    }
}