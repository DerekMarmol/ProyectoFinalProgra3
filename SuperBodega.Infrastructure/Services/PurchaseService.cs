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
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IProductRepository _productRepository;

        public PurchaseService(IPurchaseRepository purchaseRepository, IProductRepository productRepository)
        {
            _purchaseRepository = purchaseRepository;
            _productRepository = productRepository;
        }

        public async Task<PurchaseDto> CreatePurchaseAsync(PurchaseDto purchaseDto)
        {
            var purchase = new Purchase
            {
                PurchaseDate = purchaseDto.PurchaseDate,
                SupplierId = purchaseDto.SupplierId,
                TotalAmount = purchaseDto.TotalAmount,
                Reference = purchaseDto.Reference,
                PurchaseDetails = new List<PurchaseDetail>()
            };

            if (purchaseDto.PurchaseDetails != null && purchaseDto.PurchaseDetails.Count > 0)
            {
                foreach (var detailDto in purchaseDto.PurchaseDetails)
                {
                    var detail = new PurchaseDetail
                    {
                        ProductId = detailDto.ProductId,
                        Quantity = detailDto.Quantity,
                        UnitPrice = detailDto.UnitPrice,
                        TotalPrice = detailDto.TotalPrice
                    };

                    purchase.PurchaseDetails.Add(detail);
                    
                    // Actualizar stock del producto
                    await _productRepository.UpdateStockAsync(detailDto.ProductId, detailDto.Quantity);
                }
            }

            await _purchaseRepository.AddAsync(purchase);

            purchaseDto.Id = purchase.Id;
            return await GetPurchaseByIdAsync(purchase.Id);
        }

        public async Task DeletePurchaseAsync(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase != null)
            {
                await _purchaseRepository.DeleteAsync(purchase);
            }
        }

        public async Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync()
        {
            var purchases = await _purchaseRepository.GetAllAsync();
            
            return purchases.Select(MapPurchaseToDto);
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await _purchaseRepository.GetPurchasesByDateRangeAsync(startDate, endDate);
            
            return purchases.Select(MapPurchaseToDto);
        }

        public async Task<PurchaseDto> GetPurchaseByIdAsync(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
                return null;

            return MapPurchaseToDto(purchase);
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesBySupplierAsync(int supplierId)
        {
            var purchases = await _purchaseRepository.GetPurchasesBySupplierAsync(supplierId);
            
            return purchases.Select(MapPurchaseToDto);
        }

        public async Task UpdatePurchaseAsync(PurchaseDto purchaseDto)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseDto.Id);
            if (purchase != null)
            {
                purchase.PurchaseDate = purchaseDto.PurchaseDate;
                purchase.SupplierId = purchaseDto.SupplierId;
                purchase.TotalAmount = purchaseDto.TotalAmount;
                purchase.Reference = purchaseDto.Reference;

                await _purchaseRepository.UpdateAsync(purchase);
            }
        }

        private PurchaseDto MapPurchaseToDto(Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.Name,
                TotalAmount = purchase.TotalAmount,
                Reference = purchase.Reference,
                PurchaseDetails = purchase.PurchaseDetails?.Select(pd => new PurchaseDetailDto
                {
                    Id = pd.Id,
                    PurchaseId = pd.PurchaseId,
                    ProductId = pd.ProductId,
                    ProductName = pd.Product?.Name,
                    Quantity = pd.Quantity,
                    UnitPrice = pd.UnitPrice,
                    TotalPrice = pd.TotalPrice
                }).ToList()
            };
        }
    }
}