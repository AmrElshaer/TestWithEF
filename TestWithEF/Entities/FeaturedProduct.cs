namespace TestWithEF.Entities;

public class FeaturedProduct:Product
{
    public DateTimeOffset Start { get;private set; }
    public DateTimeOffset End { get;private set; }
    public FeaturedProduct(Guid id,string name,DateTimeOffset start,DateTimeOffset end) : base(id,name, ProductType.Featured)
    {
        Start = start;
        End = end;
    }
}
 