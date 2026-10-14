namespace VyshKat.Application;
using VyshKat.Domain.Abstractions;
using VyshKat.Domain.Services;

public class FleetService
{
    private readonly IFleetRepository _repository;
    private readonly IServiceCenter _serviceCenter;
    public FleetService(IFleetRepository repository, IServiceCenter serviceCenter)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    public bool TryAccept(IInventoryItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (!_serviceCenter.Approve(item))
        {
            return false;
        }

        if (_repository.GetByInventoryNumber(item.InventoryNumber) != null)
        {
            return false;
        }
        
        _repository.Add(item);
        return true;
    }

    public IInventoryItem? GetItem(string inventoryNumber) => _repository.GetByInventoryNumber(inventoryNumber);
    public IEnumerable<IInventoryItem> GetAllItems() => _repository.GetAll();
}