namespace VyshKat.Domain.Gear;
using VyshKat.Domain.Abstractions;

public class ChargingCabinet : GearItem, IEnergyConsumer
{
    public decimal DailyEnergyConsumptionKwh { get; }
    
    public ChargingCabinet(string inventoryNumber, string name, decimal energyConsumption) : base(inventoryNumber, name)
    {
        DailyEnergyConsumptionKwh = energyConsumption;
    }
}