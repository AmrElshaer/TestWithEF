using TestWithEF.Entities;
using TestWithEF.IRepositories.Base;

namespace TestWithEF.IRepositories;

public interface IFeaturedProductRepository: IRepository<FeaturedProduct, Guid>
{
    
}
