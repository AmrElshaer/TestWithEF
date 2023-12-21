using MediatR;
using Microsoft.EntityFrameworkCore;
using TestWithEF.Exceptions;
using TestWithEF.Models;

namespace TestWithEF.Features.Authors.Queries.GetAuthorById;

public sealed record GetAuthorByIdQuery(Guid Id) : IRequest<Result<GetAuthorByIdDto>>;

public sealed class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Result<GetAuthorByIdDto>>
{
    private readonly TestDbContext _db;

    public GetAuthorByIdQueryHandler(TestDbContext db) => _db = db;

    public async Task<Result<GetAuthorByIdDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        return (await GetAuthor(request, cancellationToken)).ToResult(NotFoundException.ForAuthor(request.Id));
    }

    private async Task<Maybe<GetAuthorByIdDto>> GetAuthor(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Authors.Where(author => author.Id == request.Id).Select(author => new GetAuthorByIdDto
        {
            Id = author.Id,
            Name = author.Name,
            Phone = author.ContactDetails.Phone,
            Street = author.ContactDetails.Address.Street,
            City = author.ContactDetails.Address.City,
            Postcode = author.ContactDetails.Address.Postcode,
            Country = author.ContactDetails.Address.Country
        }).SingleOrDefaultAsync(cancellationToken);
    }
}
