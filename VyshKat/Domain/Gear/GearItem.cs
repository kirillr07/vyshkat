namespace VyshKat.Domain.Gear;
using VyshKat.Domain.Abstractions;

public abstract class GearItem : IInventoryItem
{
    public string InventoryNumber { get; }
    public string Name { get; }

    protected GearItem(string inventoryNumber, string name)
    {
        InventoryNumber = inventoryNumber;
        Name = name;
    }
}