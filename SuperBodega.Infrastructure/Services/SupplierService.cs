using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<SupplierDto> CreateSupplierAsync(SupplierDto supplierDto)
        {
            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                ContactName = supplierDto.ContactName,
                Email = supplierDto.Email,
                Phone = supplierDto.Phone,
                Address = supplierDto.Address
            };

            await _supplierRepository.AddAsync(supplier);

            supplierDto.Id = supplier.Id;
            return supplierDto;
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier != null)
            {
                await _supplierRepository.DeleteAsync(supplier);
            }
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            
            return suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactName = s.ContactName,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address
            });
        }

        public async Task<SupplierDto> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return null;

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address
            };
        }

        public async Task UpdateSupplierAsync(SupplierDto supplierDto)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierDto.Id);
            if (supplier != null)
            {
                supplier.Name = supplierDto.Name;
                supplier.ContactName = supplierDto.ContactName;
                supplier.Email = supplierDto.Email;
                supplier.Phone = supplierDto.Phone;
                supplier.Address = supplierDto.Address;

                await _supplierRepository.UpdateAsync(supplier);
            }
        }
    }
}