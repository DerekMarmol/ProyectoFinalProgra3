using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Infrastructure.Data;

namespace SuperBodega.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}