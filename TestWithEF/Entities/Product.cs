using CSharpFunctionalExtensions;

namespace TestWithEF.Entities;

public abstract class Product:Entity<Guid>
{
    public string Name { get;private set; }
    public ProductType ProductType { get;private set; }

    protected Product(Guid id,string name, ProductType productType):base(id)
    {
        Name = name;
        ProductType = productType;
    }
    
}

public enum ProductType:byte
{
    Standard=1,  
    Featured
}
