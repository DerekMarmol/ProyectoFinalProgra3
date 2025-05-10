using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Infrastructure.Data;

namespace SuperBodega.Infrastructure.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<CartItem>> GetCartItemsByCartIdAsync(string cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId && ci.IsActive)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        public async Task ClearCartAsync(string cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}