using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetAll()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDto>> GetById(int id)
        {
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
            if (purchase == null)
                return NotFound();

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseDto>> Create(PurchaseDto purchaseDto)
        {
            var createdPurchase = await _purchaseService.CreatePurchaseAsync(purchaseDto);
            return CreatedAtAction(nameof(GetById), new { id = createdPurchase.Id }, createdPurchase);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PurchaseDto purchaseDto)
        {
            if (id != purchaseDto.Id)
                return BadRequest();

            await _purchaseService.UpdatePurchaseAsync(purchaseDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _purchaseService.DeletePurchaseAsync(id);
            return NoContent();
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetBySupplier(int supplierId)
        {
            var purchases = await _purchaseService.GetPurchasesBySupplierAsync(supplierId);
            return Ok(purchases);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var purchases = await _purchaseService.GetPurchasesByDateRangeAsync(startDate, endDate);
            return Ok(purchases);
        }
    }
}