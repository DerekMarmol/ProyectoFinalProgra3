using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/async/sales")]
    public class AsyncSalesController : ControllerBase
    {
        private readonly IAsyncSaleService _asyncSaleService;
        private readonly ISaleService _saleService;

        public AsyncSalesController(IAsyncSaleService asyncSaleService, ISaleService saleService)
        {
            _asyncSaleService = asyncSaleService;
            _saleService = saleService;
        }

        [HttpPost]
        public async Task<ActionResult<SaleDto>> Create(SaleDto saleDto)
        {
            var createdSale = await _asyncSaleService.CreateSaleAsync(saleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSale.Id }, createdSale);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] SaleStatus status)
        {
            var result = await _asyncSaleService.UpdateSaleStatusAsync(id, status);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}