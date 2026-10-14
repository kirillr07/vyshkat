namespace VyshKat.Domain.Vehicles;
using VyshKat.Domain.Abstractions;

public class ElectricBicycle : Vehicle, INoviceFriendly, IEnergyConsumer
{
    public int NoviceRating {get;}
    public decimal DailyEnergyConsumptionKwh { get; }

    public ElectricBicycle(string inventoryNumber, string name, int noviceRating, decimal energyConsumption)
        : base (inventoryNumber, name)
    {
        NoviceRating = noviceRating;
        DailyEnergyConsumptionKwh = energyConsumption;
    }
}