using MediatR;
using Microsoft.EntityFrameworkCore;

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
        return await _db.Orders.Select(o => new GetAllOrdersDto()
        {
            Description = o.Description,
            OrderProducts = o.OrderProducts.Select(op => new GetAllOrderProductDto()
            {
                ProductId = op.ProductId,
                Quantity = op.Quantity,
            })
        }).ToListAsync(cancellationToken);
    }
}
