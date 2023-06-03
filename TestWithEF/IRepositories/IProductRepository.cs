using TestWithEF.Entities;
using TestWithEF.IRepositories.Base;

namespace TestWithEF.IRepositories;

public interface IProductRepository: IRepository<Product, Guid>
{
    
}
