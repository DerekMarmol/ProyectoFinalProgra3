using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Infrastructure.Data;

namespace SuperBodega.Infrastructure.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Sale>> GetSalesByCustomerAsync(int customerId)
        {
            return await _context.Sales
                .Where(s => s.CustomerId == customerId && s.IsActive)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.IsActive)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Sale>> GetSalesByProductAsync(int productId)
        {
            return await _context.Sales
                .Where(s => s.SaleDetails.Any(sd => sd.ProductId == productId) && s.IsActive)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Sale>> GetSalesBySupplierAsync(int supplierId)
        {
            return await _context.Sales
                .Where(s => s.SaleDetails.Any(sd => sd.Product.SupplierId == supplierId) && s.IsActive)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int saleId, SaleStatus status)
        {
            var sale = await _context.Sales.FindAsync(saleId);
            if (sale == null)
                return false;

            sale.Status = status;
            sale.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}