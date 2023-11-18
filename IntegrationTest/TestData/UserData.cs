using Bogus;
using TestWithEF.Models;

namespace IntegrationTest.TestData;

public  static partial class DataGenerator
{
    public static Faker<RegisterModel> RegisterUserRequest()
    {
        var faker = new Faker<RegisterModel>()
            .RuleFor(x => x.Username, f => f.Person.Email)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Password, f => f.Internet.Password(prefix:"P@ssw0rd"));

        return faker;
    }
}
