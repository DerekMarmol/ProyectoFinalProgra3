using System.Collections.Generic;
using System.Threading.Tasks;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IReadOnlyList<Product>> GetProductsBySupplierAsync(int supplierId);
        Task UpdateStockAsync(int productId, int quantity);
    }
}