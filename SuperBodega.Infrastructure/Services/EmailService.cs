using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendOrderConfirmationAsync(SaleDto sale, string customerEmail)
        {
            _logger.LogInformation("Iniciando envío de correo de confirmación para pedido {OrderId} a {Email}", 
                sale.Id, customerEmail);
            
            try {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Super Bodega", _configuration["Email:From"]));
                message.To.Add(new MailboxAddress(sale.CustomerName, customerEmail));
                message.Subject = $"Confirmación de pedido #{sale.Reference}";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                    <h1>¡Gracias por tu compra!</h1>
                    <p>Hemos recibido tu pedido y está siendo procesado.</p>
                    <h2>Detalles del pedido:</h2>
                    <p><strong>Número de referencia:</strong> {sale.Reference}</p>
                    <p><strong>Fecha:</strong> {sale.SaleDate:dd/MM/yyyy HH:mm}</p>
                    <p><strong>Total:</strong> {sale.TotalAmount:C}</p>
                    
                    <h3>Productos:</h3>
                    <table border='1' cellpadding='5' cellspacing='0'>
                        <tr>
                            <th>Producto</th>
                            <th>Cantidad</th>
                            <th>Precio</th>
                            <th>Total</th>
                        </tr>";

                foreach (var item in sale.SaleDetails)
                {
                    builder.HtmlBody += $@"
                        <tr>
                            <td>{item.ProductName}</td>
                            <td>{item.Quantity}</td>
                            <td>{item.UnitPrice:C}</td>
                            <td>{item.TotalPrice:C}</td>
                        </tr>";
                }

                builder.HtmlBody += $@"
                    </table>
                    <p>Te notificaremos cuando tu pedido sea enviado.</p>
                    <p>Saludos,<br>El equipo de Super Bodega</p>";

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                _logger.LogInformation("Conectando al servidor SMTP: {Server}:{Port}", 
                    _configuration["Email:SmtpServer"], _configuration["Email:Port"]);
                    
                await client.ConnectAsync(
                    _configuration["Email:SmtpServer"], 
                    int.Parse(_configuration["Email:Port"]), 
                    MailKit.Security.SecureSocketOptions.StartTls);
                
                _logger.LogInformation("Autenticando con usuario: {Username}", 
                    _configuration["Email:Username"]);
                    
                await client.AuthenticateAsync(
                    _configuration["Email:Username"], 
                    _configuration["Email:Password"]);
                
                _logger.LogInformation("Enviando mensaje...");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                
                _logger.LogInformation("Email de confirmación enviado a {Email} para el pedido {OrderId}", 
                    customerEmail, sale.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email de confirmación a {Email} para el pedido {OrderId}: {Message}", 
                    customerEmail, sale.Id, ex.Message);
                throw; // Re-lanzar para que se capture en el nivel superior
            }
        }

        public async Task SendOrderStatusUpdateAsync(int saleId, string customerName, string customerEmail, SaleStatus newStatus)
        {
            var statusText = newStatus switch
            {
                SaleStatus.Pending => "Pendiente",
                SaleStatus.Processing => "En procesamiento",
                SaleStatus.Shipped => "Enviado",
                SaleStatus.Delivered => "Entregado",
                SaleStatus.Cancelled => "Cancelado",
                _ => newStatus.ToString()
            };

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Super Bodega", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress(customerName, customerEmail));
            message.Subject = $"Actualización de tu pedido #{saleId} - {statusText}";

            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
                <h1>Actualización de tu pedido</h1>
                <p>Hola {customerName},</p>
                <p>El estado de tu pedido #{saleId} ha cambiado a: <strong>{statusText}</strong>.</p>";

            if (newStatus == SaleStatus.Shipped)
            {
                builder.HtmlBody += "<p>¡Tu pedido ha sido enviado! Pronto recibirás tu compra.</p>";
            }
            else if (newStatus == SaleStatus.Delivered)
            {
                builder.HtmlBody += "<p>¡Tu pedido ha sido entregado! Esperamos que disfrutes tus productos.</p>";
            }

            builder.HtmlBody += $@"
                <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                <p>Saludos,<br>El equipo de Super Bodega</p>";

            message.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Email:SmtpServer"], 
                    int.Parse(_configuration["Email:Port"]), 
                    MailKit.Security.SecureSocketOptions.StartTls);
                
                await client.AuthenticateAsync(
                    _configuration["Email:Username"], 
                    _configuration["Email:Password"]);
                
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                
                _logger.LogInformation("Email de actualización de estado enviado a {Email} para el pedido {OrderId}", 
                    customerEmail, saleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email de actualización de estado a {Email} para el pedido {OrderId}", 
                    customerEmail, saleId);
            }
        }
    }
}