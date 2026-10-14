namespace VyshKat.Domain.Vehicles;
using VyshKat.Domain.Abstractions;

public abstract class Vehicle : IInventoryItem
{
    public string InventoryNumber {get;}
    public string Name { get; }
    
    protected Vehicle(string inventoryNumber, string name)
    {
        InventoryNumber = inventoryNumber;
        Name = name;
    }
}