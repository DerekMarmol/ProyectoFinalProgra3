using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<CartItemDto> AddItemToCartAsync(string cartId, int productId, int quantity)
        {
            var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);
            var existingItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartItemRepository.UpdateAsync(existingItem);
                return MapCartItemToDto(existingItem);
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return null;

            var newItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity
            };

            await _cartItemRepository.AddAsync(newItem);

            return MapCartItemToDto(newItem, product);
        }

        public async Task ClearCartAsync(string cartId)
        {
            await _cartItemRepository.ClearCartAsync(cartId);
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string cartId)
        {
            var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);
            
            return cartItems.Select(ci => MapCartItemToDto(ci));
        }

        public async Task<bool> RemoveItemFromCartAsync(string cartId, int productId)
        {
            var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);
            var item = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (item == null)
                return false;

            await _cartItemRepository.DeleteAsync(item);
            return true;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(string cartId, int productId, int quantity)
        {
            var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);
            var item = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (item == null)
                return false;

            item.Quantity = quantity;
            await _cartItemRepository.UpdateAsync(item);
            return true;
        }

        private CartItemDto MapCartItemToDto(CartItem cartItem, Product product = null)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                ProductName = product?.Name ?? cartItem.Product?.Name,
                ProductPrice = product?.Price ?? cartItem.Product?.Price ?? 0,
                Quantity = cartItem.Quantity
            };
        }
    }
}