namespace TestWithEF.Entities;

public class StandardProduct:Product
{
    
    public StandardProduct(Guid id,string name) : base(id,name, ProductType.Standard)
    {
        
    }

}
