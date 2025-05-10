using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailService _emailService;

        public SaleService(
            ISaleRepository saleRepository, 
            IProductRepository productRepository,
            ICustomerRepository customerRepository,
            IEmailService emailService)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _emailService = emailService;
        }

        public async Task<SaleDto> CreateSaleAsync(SaleDto saleDto)
        {
            var sale = new Sale
            {
                SaleDate = saleDto.SaleDate,
                CustomerId = saleDto.CustomerId,
                TotalAmount = saleDto.TotalAmount,
                Status = saleDto.Status,
                Reference = saleDto.Reference,
                SaleDetails = new List<SaleDetail>()
            };

            if (saleDto.SaleDetails != null && saleDto.SaleDetails.Count > 0)
            {
                foreach (var detailDto in saleDto.SaleDetails)
                {
                    var detail = new SaleDetail
                    {
                        ProductId = detailDto.ProductId,
                        Quantity = detailDto.Quantity,
                        UnitPrice = detailDto.UnitPrice,
                        TotalPrice = detailDto.TotalPrice
                    };

                    sale.SaleDetails.Add(detail);
                    
                    // Reducir stock del producto (valor negativo)
                    await _productRepository.UpdateStockAsync(detailDto.ProductId, -detailDto.Quantity);
                }
            }

            await _saleRepository.AddAsync(sale);

            saleDto.Id = sale.Id;
            
            // Después de crear la venta, enviar correo de confirmación
            var customer = await _customerRepository.GetByIdAsync(saleDto.CustomerId);
            if (customer != null && _emailService != null)
            {
                await _emailService.SendOrderConfirmationAsync(saleDto, customer.Email);
            }
            
            return await GetSaleByIdAsync(sale.Id);
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale != null)
            {
                await _saleRepository.DeleteAsync(sale);
            }
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _saleRepository.GetAllAsync();
            
            return sales.Select(MapSaleToDto);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId)
        {
            var sales = await _saleRepository.GetSalesByCustomerAsync(customerId);
            
            return sales.Select(MapSaleToDto);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _saleRepository.GetSalesByDateRangeAsync(startDate, endDate);
            
            return sales.Select(MapSaleToDto);
        }

        public async Task<SaleDto> GetSaleByIdAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                return null;

            return MapSaleToDto(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByProductAsync(int productId)
        {
            var sales = await _saleRepository.GetSalesByProductAsync(productId);
            
            return sales.Select(MapSaleToDto);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesBySupplierAsync(int supplierId)
        {
            var sales = await _saleRepository.GetSalesBySupplierAsync(supplierId);
            
            return sales.Select(MapSaleToDto);
        }

        public async Task UpdateSaleAsync(SaleDto saleDto)
        {
            var sale = await _saleRepository.GetByIdAsync(saleDto.Id);
            if (sale != null)
            {
                sale.SaleDate = saleDto.SaleDate;
                sale.CustomerId = saleDto.CustomerId;
                sale.TotalAmount = saleDto.TotalAmount;
                sale.Status = saleDto.Status;
                sale.Reference = saleDto.Reference;

                await _saleRepository.UpdateAsync(sale);
            }
        }

        public async Task<bool> UpdateSaleStatusAsync(int saleId, SaleStatus status)
        {
            return await _saleRepository.UpdateStatusAsync(saleId, status);
        }

        private SaleDto MapSaleToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleDate = sale.SaleDate,
                CustomerId = sale.CustomerId,
                CustomerName = $"{sale.Customer?.FirstName} {sale.Customer?.LastName}",
                TotalAmount = sale.TotalAmount,
                Status = sale.Status,
                Reference = sale.Reference,
                SaleDetails = sale.SaleDetails?.Select(sd => new SaleDetailDto
                {
                    Id = sd.Id,
                    SaleId = sd.SaleId,
                    ProductId = sd.ProductId,
                    ProductName = sd.Product?.Name,
                    Quantity = sd.Quantity,
                    UnitPrice = sd.UnitPrice,
                    TotalPrice = sd.TotalPrice
                }).ToList()
            };
        }
    }
}