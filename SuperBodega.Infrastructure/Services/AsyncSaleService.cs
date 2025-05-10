using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Messaging;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class AsyncSaleService : IAsyncSaleService
    {
        private readonly ISaleService _saleService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<AsyncSaleService> _logger;

        public AsyncSaleService(ISaleService saleService, IMessagePublisher messagePublisher, ILogger<AsyncSaleService> logger)
        {
            _saleService = saleService;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<SaleDto> CreateSaleAsync(SaleDto saleDto)
        {
            var createdSale = await _saleService.CreateSaleAsync(saleDto);

            var message = new SaleCreatedMessage
            {
                Id = createdSale.Id,
                SaleDate = createdSale.SaleDate,
                CustomerId = createdSale.CustomerId,
                CustomerName = createdSale.CustomerName,
                CustomerEmail = "customer@example.com", // En una aplicación real, esto vendría del servicio de clientes
                TotalAmount = createdSale.TotalAmount,
                Status = createdSale.Status,
                Reference = createdSale.Reference,
                SaleDetails = createdSale.SaleDetails
            };

            await _messagePublisher.PublishAsync("sale.created", message);
            _logger.LogInformation("Sale {SaleId} created and message published", createdSale.Id);

            return createdSale;
        }

        public async Task<bool> UpdateSaleStatusAsync(int saleId, SaleStatus status)
        {
            var sale = await _saleService.GetSaleByIdAsync(saleId);
            if (sale == null)
                return false;

            var oldStatus = sale.Status;
            var result = await _saleService.UpdateSaleStatusAsync(saleId, status);

            if (result)
            {
                var message = new SaleStatusChangedMessage
                {
                    SaleId = saleId,
                    CustomerName = sale.CustomerName,
                    CustomerEmail = "customer@example.com", // En una aplicación real, esto vendría del servicio de clientes
                    OldStatus = oldStatus,
                    NewStatus = status,
                    StatusChangeDate = DateTime.UtcNow
                };

                await _messagePublisher.PublishAsync("sale.status.changed", message);
                _logger.LogInformation("Sale {SaleId} status changed from {OldStatus} to {NewStatus}", saleId, oldStatus, status);
            }

            return result;
        }
    }
}