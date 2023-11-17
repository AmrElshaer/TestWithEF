using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Models;

namespace TestWithEF.EndPoints.Product.CreateProduct;

public class CreateProductEndPoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/product", async (CreateProductVM request,
                IProductRepository productRepository) =>
            {
                Entities.Product product = null;
                if (request.ProductType is ProductType.Standard)
                {
                    product = new StandardProduct(Guid.NewGuid(),request.Name);
                }
                else
                {
                    product = new FeaturedProduct(Guid.NewGuid(),request.Name, request.Start, request.End);
                }

                await productRepository.AddAsync(product);
                return Results.Ok(product.Id);
            })
            .WithName("Create Product")
            .WithSummary("Create Product")
            .WithDescription("Create Product")
            .WithApiVersionSet(builder.NewApiVersionSet("Product").Build())
            .Produces<Guid>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
