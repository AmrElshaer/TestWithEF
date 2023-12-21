using MediatR;
using TestWithEF.Entities;
using TestWithEF.Models;
using TestWithEF.ValueObjects;

namespace TestWithEF.Features.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(string Name, string Postcode, string Street, string City, string Country, string Phone) : IRequest<Result<Guid>>;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Result<Guid>>
{
    private readonly TestDbContext _db;

    public CreateAuthorCommandHandler(TestDbContext db)
    {
        _db = db;
    }

    public Task<Result<Guid>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        => CreateAuthor(request).MapAsync(a => Persist(a, cancellationToken))
            .MapAsync(a => a.Id);

    private async Task<Author> Persist(Author author, CancellationToken cancellationToken)
    {
        await _db.Authors.AddAsync(author, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return author;
    }

    private static Result<Author> CreateAuthor(CreateAuthorCommand request)
    {
        return (CreateContactDetails(request), CreateAuthorName(request))
            .Apply((contactDetails, authorName) => Author.CreateAuthor(authorName, contactDetails));
    }

    private static Result<AuthorName> CreateAuthorName(CreateAuthorCommand request)
    {
        return AuthorName.CreateAuthorName(request.Name);
    }

    private static Result<ContactDetails> CreateContactDetails(CreateAuthorCommand request)
    {
        return CreateAddress(request).Bind(a => ContactDetails.CreateContactDetails(request.Phone, a));
    }

    private static Result<Address> CreateAddress(CreateAuthorCommand request)
    {
        return Address.CreateAddress(request.Street,
            request.City,
            request.Postcode,
            request.Country);
    }
}
