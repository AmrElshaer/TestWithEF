using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Repository.Base;

namespace TestWithEF.Repository;

public class StandardProductRepository: Repository<StandardProduct, Guid>, IStandardProductRepository
{
    public StandardProductRepository(TestContext dbContext): base(dbContext)
    {
    }
}
