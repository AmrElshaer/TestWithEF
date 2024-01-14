using MediatR;
using Microsoft.EntityFrameworkCore;
using static TestWithEF.Features.Orders.Queries.GetAllOrders.GetAllOrdersDtoMapper;

namespace TestWithEF.Features.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery : IRequest<IReadOnlyList<GetAllOrdersDto>>;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IReadOnlyList<GetAllOrdersDto>>
{
    private readonly TestDbContext _db;

    public GetAllOrdersQueryHandler(TestDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<GetAllOrdersDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Orders.Select(MapTo()).ToListAsync(cancellationToken);
    }
}
