namespace VyshKat.Domain.Services;
using VyshKat.Domain.Abstractions;

public class ServiceCenter : IServiceCenter
{
    public bool Approve(IInventoryItem item)
    {
        if (item == null) return false;
        if (string.IsNullOrWhiteSpace(item.InventoryNumber)) return false;
        if (string.IsNullOrWhiteSpace(item.Name)) return false;

        if (item is IEnergyConsumer consumer && consumer.DailyEnergyConsumptionKwh < 0)
        {
            return false;
        }
        
        if (item is INoviceFriendly novice && (novice.NoviceRating < 1 || novice.NoviceRating > 10))
        {
            return false;
        }

        return true;
    }
}