using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ISaleService _saleService;
        private readonly ILogger<NotificationsController> _logger;
        
        public NotificationsController(
            IEmailService emailService, 
            ISaleService saleService,
            ILogger<NotificationsController> logger)
        {
            _emailService = emailService;
            _saleService = saleService;
            _logger = logger;
        }
        
        [HttpPost("send-order-confirmation")]
        public async Task<IActionResult> SendOrderConfirmation([FromBody] OrderConfirmationRequest request)
        {
            _logger.LogInformation("Recibida solicitud para enviar confirmación de pedido {SaleId} a {Email}", 
                request.SaleId, request.CustomerEmail);
                
            try
            {
                var sale = await _saleService.GetSaleByIdAsync(request.SaleId);
                if (sale == null)
                {
                    _logger.LogWarning("Venta no encontrada: {SaleId}", request.SaleId);
                    return NotFound(new { error = $"No se encontró la venta con ID {request.SaleId}" });
                }
                    
                await _emailService.SendOrderConfirmationAsync(sale, request.CustomerEmail);
                _logger.LogInformation("Confirmación de pedido enviada con éxito para la venta {SaleId}", request.SaleId);
                return Ok(new { message = "Correo de confirmación enviado con éxito" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la confirmación de pedido {SaleId}: {Message}", 
                    request.SaleId, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class OrderConfirmationRequest
    {
        public int SaleId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
    }
}