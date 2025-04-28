using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<SaleDto> GetSaleByIdAsync(int id);
        Task<SaleDto> CreateSaleAsync(SaleDto saleDto);
        Task UpdateSaleAsync(SaleDto saleDto);
        Task DeleteSaleAsync(int id);
        Task<bool> UpdateSaleStatusAsync(int saleId, SaleStatus status);
        Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId);
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleDto>> GetSalesByProductAsync(int productId);
        Task<IEnumerable<SaleDto>> GetSalesBySupplierAsync(int supplierId);
    }
}