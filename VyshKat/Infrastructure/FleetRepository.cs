namespace VyshKat.Infrastructure;
using VyshKat.Domain.Abstractions;

public class FleetRepository : IFleetRepository
{
    private readonly Dictionary<string, IInventoryItem> _items = new();

    public void Add(IInventoryItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        _items[item.InventoryNumber] = item;
    }

    public IInventoryItem? GetByInventoryNumber(string inventoryNumber)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentNullException(nameof(inventoryNumber));

        return _items.TryGetValue(inventoryNumber, out var item) ? item : null;
    }

    public IEnumerable<IInventoryItem> GetAll()
    {
        return _items.Values;
    }
}