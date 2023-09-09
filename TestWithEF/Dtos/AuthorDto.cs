using TestWithEF.Entities;

namespace TestWithEF.Dtos;

public class AuthorDto : BaseDto<AuthorDto, Author>
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string Phone { get; init; }

    public string Street { get; init; }

    public string City { get; init; }

    public string Postcode { get; init; }

    public string Country { get; init; }

    public override void AddCustomMappings()
    {
        SetCustomMappingsInverse()
            .Map(des => des.Phone, src => src.ContactDetails.Phone)
            .Map(des => des.Street, src => src.ContactDetails.Address.Street)
            .Map(des => des.Postcode, src => src.ContactDetails.Address.Postcode)
            .Map(des => des.City, src => src.ContactDetails.Address.City)
            .Map(des => des.Country, src => src.ContactDetails.Address.Country);
    }
}
