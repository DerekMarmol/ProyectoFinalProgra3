using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(SaleDto sale, string customerEmail);
        Task SendOrderStatusUpdateAsync(int saleId, string customerName, string customerEmail, SaleStatus newStatus);
    }
}