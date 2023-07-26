using TestWithEF.Entities;

namespace TestWithEF.Dtos;

public  record struct AuthorDto(Guid Id, string Name, string Phone, string Street, string City, string Postcode, string Country)
{
    public static implicit operator AuthorDto(Author author)
    {
        return new ()
        {
            Id = author.Id,
            Name = author.Name,
            Phone = author.ContactDetails.Phone,
            Street = author.ContactDetails.Address.Street,
            City = author.ContactDetails.Address.City,
            Postcode = author.ContactDetails.Address.Postcode,
            Country = author.ContactDetails.Address.Country
            
        };
    }
}


