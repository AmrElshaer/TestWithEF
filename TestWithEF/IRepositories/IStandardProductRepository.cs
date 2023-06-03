using TestWithEF.Entities;
using TestWithEF.IRepositories.Base;

namespace TestWithEF.IRepositories;

public interface IStandardProductRepository: IRepository<StandardProduct, Guid>
{
    
}
