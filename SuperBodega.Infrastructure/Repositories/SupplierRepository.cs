using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Infrastructure.Data;

namespace SuperBodega.Infrastructure.Repositories
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}