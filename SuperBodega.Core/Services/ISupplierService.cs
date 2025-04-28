using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;

namespace SuperBodega.Core.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto> GetSupplierByIdAsync(int id);
        Task<SupplierDto> CreateSupplierAsync(SupplierDto supplierDto);
        Task UpdateSupplierAsync(SupplierDto supplierDto);
        Task DeleteSupplierAsync(int id);
    }
}