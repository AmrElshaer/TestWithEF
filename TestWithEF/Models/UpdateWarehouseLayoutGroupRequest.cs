namespace TestWithEF.Models;

public class UpdateWarehouseLayoutGroupRequest
{
    public string Name { get; init; }

    public IList<Guid> Subcategories { get; init; }
}
