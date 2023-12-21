using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TestWithEF.Features.Authors.Queries.GetAuthors;

public sealed record GetAuthorsQuery : IRequest<IReadOnlyList<GetAllAuthorsDto>>;

public sealed class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, IReadOnlyList<GetAllAuthorsDto>>
{
    private readonly TestDbContext _db;

    public GetAuthorsQueryHandler(TestDbContext db) => _db = db;

    public async Task<IReadOnlyList<GetAllAuthorsDto>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Authors.Select(author => new GetAllAuthorsDto
        {
            Id = author.Id,
            Name = author.Name,
            Phone = author.ContactDetails.Phone,
            Street = author.ContactDetails.Address.Street,
            City = author.ContactDetails.Address.City,
            Postcode = author.ContactDetails.Address.Postcode,
            Country = author.ContactDetails.Address.Country
        }).ToListAsync(cancellationToken);
    }
}
