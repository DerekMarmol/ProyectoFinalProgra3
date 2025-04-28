using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductService _productService;

        public CatalogController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetAllProductsAsync();
            
            // Implementar paginación básica (en una aplicación real, usaríamos algo más sofisticado)
            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            return Ok(pagedProducts);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            
            // Implementar paginación básica
            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            return Ok(pagedProducts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            
            return Ok(product);
        }
    }
}