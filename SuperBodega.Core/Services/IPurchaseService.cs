using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;

namespace SuperBodega.Core.Services
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync();
        Task<PurchaseDto> GetPurchaseByIdAsync(int id);
        Task<PurchaseDto> CreatePurchaseAsync(PurchaseDto purchaseDto);
        Task UpdatePurchaseAsync(PurchaseDto purchaseDto);
        Task DeletePurchaseAsync(int id);
        Task<IEnumerable<PurchaseDto>> GetPurchasesBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}