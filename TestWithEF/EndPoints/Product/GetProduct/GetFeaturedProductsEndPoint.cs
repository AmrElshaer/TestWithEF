using TestWithEF.Entities;
using TestWithEF.IRepositories;

namespace TestWithEF.EndPoints.Product.GetProduct;

public class GetFeaturedProductsEndPoint:IMinimalEndpoint
{
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/get-feature-products", async (
                IFeaturedProductRepository featurnedProductRepository,
                CancellationToken cancellationToken) =>
            {
                var featuredProducts = await featurnedProductRepository.GetAllAsync();
                return Results.Ok(featuredProducts);
            })
            .WithName("Get Featured Products")
            .WithSummary("Get Featured Products")
            .WithDescription("Get Featured Products")
            .WithApiVersionSet(builder.NewApiVersionSet("Product").Build())
            .Produces<IReadOnlyList<FeaturedProduct>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
