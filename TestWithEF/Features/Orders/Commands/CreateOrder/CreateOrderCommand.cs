using MediatR;
using Rebus.Bus;
using TestWithEF.Entities;

namespace TestWithEF.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(string Description, List<CreateOrderProductDto> OrderProducts, Guid AuthorId) : IRequest<Guid>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly TestDbContext _testDbContext;
   // private readonly IBus _bus;

    public CreateOrderCommandHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
       // _bus = bus;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.Description, request.OrderProducts.Select(op =>
                OrderProduct.Create(op.ProductId, op.Quantity)).ToList(),
            request.AuthorId);

        await _testDbContext.Orders.AddAsync(order, cancellationToken);
        await _testDbContext.SaveChangesAsync(cancellationToken);
       // await _bus.Send(new OrderCreatedEvent(order.Id));
        return order.Id;
    }
}
