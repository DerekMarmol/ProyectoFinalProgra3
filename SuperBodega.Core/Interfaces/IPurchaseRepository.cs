using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Interfaces
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId);
        Task<IReadOnlyList<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}