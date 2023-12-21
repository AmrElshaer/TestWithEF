using AutoBogus;
using Bogus;
using TestWithEF.Entities;
using TestWithEF.Features.Authors.Commands.CreateAuthor;
using TestWithEF.Features.Authors.Commands.UpdateAuthor;
using TestWithEF.ValueObjects;

namespace IntegrationTest.TestData;

public static partial class DataGenerator
{
    public static Faker<CreateAuthorCommand> CreateAuthorRequest()
    {
        var faker = new AutoFaker<CreateAuthorCommand>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Street, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, faker => faker.Address.City())
            .RuleFor(x => x.Postcode, faker => faker.Address.ZipCode())
            .RuleFor(x => x.Country, faker => faker.Address.Country())
            .RuleFor(x => x.Phone, faker => faker.Phone.PhoneNumber());

        return faker;
    }
    public static Faker<UpdateAuthorCommand> UpdateAuthorRequest(Guid authorId)
    {
        var faker = new AutoFaker<UpdateAuthorCommand>()
            .RuleFor(x=>x.Id,f=>authorId)
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Street, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, faker => faker.Address.City())
            .RuleFor(x => x.Postcode, faker => faker.Address.ZipCode())
            .RuleFor(x => x.Country, faker => faker.Address.Country())
            .RuleFor(x => x.Phone, faker => faker.Phone.PhoneNumber());

        return faker;
    }

    public static Author CreateAuthor()
    {
        var createContactDetails =  ContactDetails.CreateContactDetails("4515151",
            Address.CreateAddress("slkdfjk","skldf","lsfd","couit").Value).Value;

        var author = Author.CreateAuthor(AuthorName.CreateAuthorName("amr").Value,
            createContactDetails);

        return author;
    }
}
