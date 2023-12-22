namespace TestWithEF.Features.Orders.Queries.GetAllOrders;

public record struct GetAllOrdersDto(string Description,IEnumerable<GetAllOrderProductDto> OrderProducts);

public record struct GetAllOrderProductDto(Guid ProductId,int Quantity);
