namespace VyshKat.Domain.Abstractions;

public interface IFleetRepository
{
    void Add(IInventoryItem item);
    IInventoryItem? GetByInventoryNumber(string inventoryNumber);
    IEnumerable<IInventoryItem> GetAll();
}