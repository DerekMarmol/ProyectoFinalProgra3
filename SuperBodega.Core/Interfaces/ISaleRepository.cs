using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Interfaces
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IReadOnlyList<Sale>> GetSalesByCustomerAsync(int customerId);
        Task<IReadOnlyList<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IReadOnlyList<Sale>> GetSalesByProductAsync(int productId);
        Task<IReadOnlyList<Sale>> GetSalesBySupplierAsync(int supplierId);
        Task<bool> UpdateStatusAsync(int saleId, SaleStatus status);
    }
}