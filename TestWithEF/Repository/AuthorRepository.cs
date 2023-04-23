using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Repository.Base;

namespace TestWithEF.Repository
{
    public class AuthorRepository : Repository<Author, Guid>, IAuthorRepository
    {
        public AuthorRepository(TestContext dbContext) : base(dbContext)
        {
        }
    }
}
