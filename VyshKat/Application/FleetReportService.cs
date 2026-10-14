namespace VyshKat.Application;
using VyshKat.Domain.Abstractions;

public class FleetReportService
{
    private readonly IFleetRepository _repository;
    public FleetReportService(IFleetRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IEnumerable<IEnergyConsumer> GetEnergyConsumers()
    {
        return _repository.GetAll().OfType<IEnergyConsumer>();
    }

    public IEnumerable<INoviceFriendly> GetNoviceFriendlyItems()
    {
        return _repository.GetAll().OfType<INoviceFriendly>().Where(item => item.IsSuitableForNovice);
    }

    public decimal CalculateTotalDailyEnergyConsumption()
    {
        return GetEnergyConsumers()
            .Sum(consumer => consumer.DailyEnergyConsumptionKwh);
    }

    public Dictionary<string, int> GetItemsCountByType()
    {
        return _repository.GetAll().GroupBy(item => item.GetType().Name).ToDictionary(g => g.Key, g => g.Count());
    }
}