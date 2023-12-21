using TestWithEF.Models;

namespace TestWithEF.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    private Email(string value) { this.Value = value; }

    public static Result<Email> CreateEmail(string email)
    {
        return email.NotEmpty()
            .Bind(e => e.ValidEmail())
            .Map(e => new Email(e));
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}
