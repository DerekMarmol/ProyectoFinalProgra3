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
    public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Purchases
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate && p.IsActive)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId)
        {
            return await _context.Purchases
                .Where(p => p.SupplierId == supplierId && p.IsActive)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .ToListAsync();
        }
    }
}