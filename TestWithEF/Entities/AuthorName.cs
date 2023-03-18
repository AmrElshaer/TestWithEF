using CSharpFunctionalExtensions;

namespace TestWithEF.Entities
{
    public class AuthorName : ValueObject
    {
        public string Value { get; private set; }
        private AuthorName() { }

        public static Result<AuthorName> CreateAuthorName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Result.Failure<AuthorName>("the author name is empty");
            return Result.Success(new AuthorName() { Value = name });
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public static implicit operator string(AuthorName authorName)
        {
            return authorName.Value;
        }
        public static explicit operator AuthorName(string authorName)
        {
            return new AuthorName() { Value = authorName };
        }
    }
}
