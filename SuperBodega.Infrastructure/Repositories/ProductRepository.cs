using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Infrastructure.Data;

namespace SuperBodega.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsBySupplierAsync(int supplierId)
        {
            return await _context.Products
                .Where(p => p.SupplierId == supplierId && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.Stock += quantity;
                await _context.SaveChangesAsync();
            }
        }
    }
}