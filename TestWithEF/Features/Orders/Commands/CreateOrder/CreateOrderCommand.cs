using MediatR;
using TestWithEF.Entities;

namespace TestWithEF.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(string Description, List<CreateOrderProductDto> OrderProducts) : IRequest<Guid>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly TestDbContext _testDbContext;

    public CreateOrderCommandHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.Description, request.OrderProducts.Select(op =>
            OrderProduct.Create(op.ProductId, op.Quantity)).ToList());

        await _testDbContext.Orders.AddAsync(order, cancellationToken);
        await _testDbContext.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
