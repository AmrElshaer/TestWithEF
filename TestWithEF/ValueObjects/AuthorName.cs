using TestWithEF.Models;

namespace TestWithEF.ValueObjects
{
    public class AuthorName : ValueObject
    {
        public string Value { get; private set; }

        private AuthorName() { }

        public static Result<AuthorName> CreateAuthorName(string name)
        {
            return name.NotEmpty().Map(n => new AuthorName()
            {
                Value = n
            });
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(AuthorName authorName)
        {
            return authorName.Value;
        }

        public static explicit operator AuthorName(string authorName)
        {
            return new AuthorName()
            {
                Value = authorName
            };
        }
    }
}
