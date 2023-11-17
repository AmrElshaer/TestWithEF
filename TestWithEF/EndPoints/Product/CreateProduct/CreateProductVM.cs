using TestWithEF.Entities;

namespace TestWithEF.EndPoints.Product.CreateProduct;

public class CreateProductVM
{
    public string Name { get; set; }
    public ProductType ProductType { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
