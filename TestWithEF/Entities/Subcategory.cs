using JetBrains.Annotations;

namespace TestWithEF.Entities;

public sealed class Subcategory : Entity
{
    public string Name { get; private set; }

    public Guid? WarehouseLayoutGroupId { get; private set; }

    [CanBeNull]
    public WarehouseLayoutGroup WarehouseLayoutGroup { get; private set; }

    public static Subcategory Create(Guid id, string name)
    {
        return new Subcategory()
        {
            Id = id,
            Name = name,
        };
    }
}
