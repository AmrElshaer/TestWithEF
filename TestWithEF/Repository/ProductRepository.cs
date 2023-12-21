using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Repository.Base;

namespace TestWithEF.Repository;

public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    public ProductRepository(TestDbContext dbDbContext) : base(dbDbContext) { }
}
