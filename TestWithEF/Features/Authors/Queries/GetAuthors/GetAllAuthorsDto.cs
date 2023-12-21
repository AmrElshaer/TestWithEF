namespace TestWithEF.Features.Authors.Queries.GetAuthors;

public class GetAllAuthorsDto
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string Phone { get; init; }

    public string Street { get; init; }

    public string City { get; init; }

    public string Postcode { get; init; }

    public string Country { get; init; }
}
