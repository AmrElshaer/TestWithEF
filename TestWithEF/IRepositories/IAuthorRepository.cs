using TestWithEF.Entities;
using TestWithEF.IRepositories.Base;

namespace TestWithEF.IRepositories
{
    public interface IAuthorRepository:IRepository<Author,Guid>
    {
    }
}
