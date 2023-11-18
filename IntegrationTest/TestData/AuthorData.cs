using AutoBogus;
using Bogus;
using TestWithEF.Models;

namespace IntegrationTest.TestData;

public static partial class DataGenerator
{
    public static Faker<CreateAuthor> CreateAuthorRequest()
    {
      

        var faker = new AutoFaker<CreateAuthor>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Street, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, faker => faker.Address.City())
            .RuleFor(x => x.Postcode, faker => faker.Address.ZipCode())
            .RuleFor(x => x.Country, faker => faker.Address.Country())
            .RuleFor(x => x.Phone, faker => faker.Phone.PhoneNumber());

        return faker;
    }
}
