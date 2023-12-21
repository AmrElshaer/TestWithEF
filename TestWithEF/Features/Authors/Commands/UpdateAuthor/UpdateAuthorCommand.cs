using MediatR;
using Microsoft.EntityFrameworkCore;
using TestWithEF.Entities;
using TestWithEF.Exceptions;
using TestWithEF.Models;
using TestWithEF.ValueObjects;

namespace TestWithEF.Features.Authors.Commands.UpdateAuthor;

public record UpdateAuthorCommand
(
    Guid Id,
    string Name,
    string Street,
    string City,
    string Postcode,
    string Country,
    string Phone
) : IRequest<Result<Guid>>;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, Result<Guid>>
{
    private readonly TestDbContext _db;

    public UpdateAuthorCommandHandler(TestDbContext db)
    {
        _db = db;
    }

    public Task<Result<Guid>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        return UpdateAuthor(request, cancellationToken)
            .MapAsync(a => _db.SaveChangesAsync(cancellationToken))
            .MapAsync(a => request.Id);
    }

    private async Task<Result<Author>> UpdateAuthor(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        return (CreateContactDetails(request), CreateAuthorName(request), await EnsureAuthorExist(request.Id, cancellationToken))
            .Apply((contactDetails, authorName, author) => author.UpdateAuthor(authorName, contactDetails));
    }

    private async Task<Result<Author>> EnsureAuthorExist(Guid id, CancellationToken cancellationToken)
        => (await GetAuthor(id, cancellationToken)).ToResult(NotFoundException.ForAuthor(id));

    private async Task<Maybe<Author>> GetAuthor(Guid id, CancellationToken cancellationToken)
        => await _db.Authors.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    private static Result<AuthorName> CreateAuthorName(UpdateAuthorCommand request)
    {
        return AuthorName.CreateAuthorName(request.Name);
    }

    private static Result<ContactDetails> CreateContactDetails(UpdateAuthorCommand request)
    {
        return CreateAddress(request).Bind(a => ContactDetails.CreateContactDetails(request.Phone, a));
    }

    private static Result<Address> CreateAddress(UpdateAuthorCommand request)
    {
        return Address.CreateAddress(request.Street,
            request.City,
            request.Postcode,
            request.Country);
    }
}
