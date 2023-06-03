using TestWithEF.Entities;
using TestWithEF.IRepositories;

namespace TestWithEF.EndPoints.Product.GetProduct;

public class GetStandardProductsEndPoint:IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/get-standard-products", async (
                IStandardProductRepository standardProductRepository,
                CancellationToken cancellationToken) =>
            {
                var featuredProducts = await standardProductRepository.GetAllAsync();
                return Results.Ok(featuredProducts);
            })
            .WithName("Get Standard Products")
            .WithSummary("Get Standard Products")
            .WithDescription("Get Standard Products")
            .WithApiVersionSet(builder.NewApiVersionSet("Product").Build())
            .Produces<IReadOnlyList<StandardProduct>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
    
}
