namespace TestWithEF.Models;

public class CreateWarehouseLayoutGroupRequest
{
    public string Name { get; init; }

    public IList<Guid> Subcategories { get; init; }
}
