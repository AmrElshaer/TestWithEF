using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace TestWithEF.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    private Email() { }

    public static Result<Email> CreateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return Result.Failure<Email>("email is empty");

        if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
        {
            return Result.Failure<Email>("email is invalid");
        }

        return new Email()
        {
            Value = email
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
