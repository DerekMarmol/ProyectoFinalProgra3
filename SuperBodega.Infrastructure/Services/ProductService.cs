using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                ImageUrl = productDto.ImageUrl,
                CategoryId = productDto.CategoryId,
                SupplierId = productDto.SupplierId
            };

            await _productRepository.AddAsync(product);

            productDto.Id = product.Id;
            return productDto;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.Name
            });
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.Name
            };
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.Name
            });
        }

        public async Task<IEnumerable<ProductDto>> GetProductsBySupplierAsync(int supplierId)
        {
            var products = await _productRepository.GetProductsBySupplierAsync(supplierId);
            
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.Name
            });
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productDto.Id);
                if (product != null)
                {
                    // Loguear para depuración
                    _logger.LogInformation("Actualizando producto: {ProductId}, Nombre: {Name}, Precio: {Price}", 
                        productDto.Id, productDto.Name, productDto.Price);
                        
                    product.Name = productDto.Name;
                    product.Description = productDto.Description;
                    product.Price = productDto.Price;
                    product.Stock = productDto.Stock;
                    product.ImageUrl = productDto.ImageUrl;
                    product.CategoryId = productDto.CategoryId;
                    product.SupplierId = productDto.SupplierId;
                    await _productRepository.UpdateAsync(product);
                    _logger.LogInformation("Producto actualizado correctamente: {ProductId}", product.Id);
                }
                else
                {
                    _logger.LogWarning("No se encontró el producto con ID: {ProductId} para actualizar", productDto.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto: {ProductId}", productDto.Id);
                throw;
            }
        }
    }
}