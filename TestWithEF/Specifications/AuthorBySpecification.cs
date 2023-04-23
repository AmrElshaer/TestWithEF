using System.Linq.Expressions;
using TestWithEF.Entities;
using TestWithEF.Specifications.Base;

namespace TestWithEF.Specifications
{
    public class AuthorBySpecification : BaseSpecification<Author>
    {
        public AuthorBySpecification(string name) 
            : base(a=>a.Name.ToLower().Contains(name.ToLower()))
        {
            // you can add some include to make sense
        }
    }
}
