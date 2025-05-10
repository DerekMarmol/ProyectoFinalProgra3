using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Messaging;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class NotificationService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageConsumer _messageConsumer;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IServiceScopeFactory serviceScopeFactory,
            IMessageConsumer messageConsumer,
            ILogger<NotificationService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageConsumer = messageConsumer;
            _logger = logger;

            // Suscribirse a los mensajes
            _messageConsumer.Subscribe<SaleCreatedMessage>("sale.created", HandleSaleCreatedAsync);
            _messageConsumer.Subscribe<SaleStatusChangedMessage>("sale.status.changed", HandleSaleStatusChangedAsync);
        }

        public void Start()
        {
            _messageConsumer.StartConsuming();
            _logger.LogInformation("Notification service started");
        }

        public void Stop()
        {
            _messageConsumer.StopConsuming();
            _logger.LogInformation("Notification service stopped");
        }

        private async Task HandleSaleCreatedAsync(SaleCreatedMessage message)
        {
            _logger.LogInformation("Procesando mensaje de venta creada: {SaleId}", message.Id);
            
            try {
                var saleDto = new SaleDto
                {
                    Id = message.Id,
                    SaleDate = message.SaleDate,
                    CustomerId = message.CustomerId,
                    CustomerName = message.CustomerName,
                    TotalAmount = message.TotalAmount,
                    Status = message.Status,
                    Reference = message.Reference,
                    SaleDetails = message.SaleDetails
                };
                
                // Crear un scope para obtener el servicio scoped
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    await emailService.SendOrderConfirmationAsync(saleDto, message.CustomerEmail);
                }
                
                _logger.LogInformation("Email de confirmación enviado con éxito para: {SaleId} a {Email}", 
                    message.Id, message.CustomerEmail);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error al enviar correo para venta {SaleId}", message.Id);
            }
        }

        private async Task HandleSaleStatusChangedAsync(SaleStatusChangedMessage message)
        {
            _logger.LogInformation("Handling sale status changed message for sale {SaleId}", message.SaleId);
            
            // Crear un scope para obtener el servicio scoped
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    
                    await emailService.SendOrderStatusUpdateAsync(
                        message.SaleId,
                        message.CustomerName,
                        message.CustomerEmail,
                        message.NewStatus);
                        
                    _logger.LogInformation("Email de actualización de estado enviado con éxito para: {SaleId} a {Email}", 
                        message.SaleId, message.CustomerEmail);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error al enviar correo de actualización para venta {SaleId}", message.SaleId);
                }
            }
        }
    }
}