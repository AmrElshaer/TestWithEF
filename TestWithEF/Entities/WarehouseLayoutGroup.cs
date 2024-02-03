namespace TestWithEF.Entities;

public sealed class WarehouseLayoutGroup : Entity
{
    public string Name { get; private set; }

    private readonly List<Subcategory> _subcategories = new();

    public IReadOnlyCollection<Subcategory> Subcategories => _subcategories;

    private WarehouseLayoutGroup() { }

    public WarehouseLayoutGroup(Guid id, string name, IReadOnlyList<Subcategory> subcategories)
    {
        Name = name;
        Id = id;
        _subcategories.AddRange(subcategories);
    }

    public void Update(string name, IReadOnlyList<Subcategory> subcategories)
    {
        Name = name;

        var subcategoriesToRemove = _subcategories
            .Except(subcategories).ToList();

        foreach (var subcategory in subcategoriesToRemove)
        {
            _subcategories.Remove(subcategory);
        }
        
        var subCategoriesToAdd = subcategories
            .Except(_subcategories)
            .ToList();
        foreach (var subcategoryToAdd in subCategoriesToAdd)
        {
           _subcategories.Add(subcategoryToAdd);
        }
    }
}
