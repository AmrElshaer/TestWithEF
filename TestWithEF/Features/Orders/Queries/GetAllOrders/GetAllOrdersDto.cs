using System.Linq.Expressions;
using TestWithEF.Entities;

namespace TestWithEF.Features.Orders.Queries.GetAllOrders;

public record struct GetAllOrdersDto(string Description,IEnumerable<GetAllOrderProductDto> OrderProducts);

public record struct GetAllOrderProductDto(Guid ProductId,int Quantity);

public class GetAllOrdersDtoMapper
{
    public static Expression<Func<Order, GetAllOrdersDto>> MapTo()
    {
        return o => new GetAllOrdersDto()
        {
            Description = o.Description,
            OrderProducts = o.OrderProducts.Select(op => new GetAllOrderProductDto()
            {
                ProductId = op.ProductId,
                Quantity = op.Quantity,
            })
        };
    }
}
