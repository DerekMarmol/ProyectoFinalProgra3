using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<SaleDto>> Create(SaleDto saleDto)
        {
            var createdSale = await _saleService.CreateSaleAsync(saleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSale.Id }, createdSale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SaleDto saleDto)
        {
            if (id != saleDto.Id)
                return BadRequest();

            await _saleService.UpdateSaleAsync(saleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _saleService.DeleteSaleAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] SaleStatus status)
        {
            var result = await _saleService.UpdateSaleStatusAsync(id, status);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetByCustomer(int customerId)
        {
            var sales = await _saleService.GetSalesByCustomerAsync(customerId);
            return Ok(sales);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(sales);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetByProduct(int productId)
        {
            var sales = await _saleService.GetSalesByProductAsync(productId);
            return Ok(sales);
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetBySupplier(int supplierId)
        {
            var sales = await _saleService.GetSalesBySupplierAsync(supplierId);
            return Ok(sales);
        }
    }
}