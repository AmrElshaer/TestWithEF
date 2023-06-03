using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Repository.Base;

namespace TestWithEF.Repository;

public class FeaturedProductRepository: Repository<FeaturedProduct, Guid>, IFeaturedProductRepository
{
    public FeaturedProductRepository(TestContext dbContext) : base(dbContext)
    {
    }
}
