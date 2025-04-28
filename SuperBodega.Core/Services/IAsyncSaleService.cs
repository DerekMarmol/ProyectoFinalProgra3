using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Services
{
    public interface IAsyncSaleService
    {
        Task<SaleDto> CreateSaleAsync(SaleDto saleDto);
        Task<bool> UpdateSaleStatusAsync(int saleId, SaleStatus status);
    }
}