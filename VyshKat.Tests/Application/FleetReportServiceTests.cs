using FluentAssertions;
using Moq;
using VyshKat.Application;
using VyshKat.Domain.Abstractions;
using VyshKat.Domain.Gear;
using VyshKat.Domain.Vehicles;
using Xunit;

namespace VyshKat.Tests.Application;

public class FleetReportServiceTests
{
    private readonly Mock<IFleetRepository> _repository = new();
    private readonly FleetReportService _sut;

    public FleetReportServiceTests()
    {
        _sut = new FleetReportService(_repository.Object);
    }

    [Fact]
    public void Constructor_Throws_WhenRepositoryIsNull()
    {
        var act = () => new FleetReportService(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetEnergyConsumers_ReturnsOnlyEnergyConsumers()
    {
        var items = new IInventoryItem[]
        {
            new Scooter("SC-1", "Scooter", 8),
            new ElectricBicycle("EB-1", "E-Bike", 7, 2.5m),
            new Helmet("H-1", "Helmet"),
            new ChargingCabinet("CC-1", "Cabinet", 5m)
        };
        _repository.Setup(r => r.GetAll()).Returns(items);

        var result = _sut.GetEnergyConsumers().ToList();

        result.Should().HaveCount(2);
        result.Should().ContainSingle(c => c is ElectricBicycle);
        result.Should().ContainSingle(c => c is ChargingCabinet);
    }

    [Fact]
    public void GetNoviceFriendlyItems_ReturnsOnlyItemsWithRatingAtLeastSix()
    {
        var items = new IInventoryItem[]
        {
            new Scooter("SC-1", "Easy", 8),
            new Scooter("SC-2", "Hard", 4),
            new Bicycle("B-1", "Medium", 6),
            new Helmet("H-1", "Helmet")
        };
        _repository.Setup(r => r.GetAll()).Returns(items);

        var result = _sut.GetNoviceFriendlyItems().ToList();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(i => i.NoviceRating >= 6);
    }

    [Fact]
    public void CalculateTotalDailyEnergyConsumption_ReturnsZero_ForEmptyFleet()
    {
        _repository.Setup(r => r.GetAll()).Returns(Array.Empty<IInventoryItem>());

        var result = _sut.CalculateTotalDailyEnergyConsumption();

        result.Should().Be(0m);
    }

    [Fact]
    public void CalculateTotalDailyEnergyConsumption_SumsOnlyConsumers()
    {
        var items = new IInventoryItem[]
        {
            new ElectricBicycle("EB-1", "E-Bike", 7, 2.5m),
            new ChargingCabinet("CC-1", "Cabinet", 5m),
            new Scooter("SC-1", "Scooter", 8),
            new Helmet("H-1", "Helmet")
        };
        _repository.Setup(r => r.GetAll()).Returns(items);

        var result = _sut.CalculateTotalDailyEnergyConsumption();

        result.Should().Be(7.5m);
    }

    [Fact]
    public void GetItemsCountByType_GroupsByConcreteType()
    {
        var items = new IInventoryItem[]
        {
            new Scooter("SC-1", "S1", 8),
            new Scooter("SC-2", "S2", 7),
            new Bicycle("B-1", "B1", 6),
            new Helmet("H-1", "H1")
        };
        _repository.Setup(r => r.GetAll()).Returns(items);

        var result = _sut.GetItemsCountByType();

        result.Should().ContainKey("Scooter").WhoseValue.Should().Be(2);
        result.Should().ContainKey("Bicycle").WhoseValue.Should().Be(1);
        result.Should().ContainKey("Helmet").WhoseValue.Should().Be(1);
    }
}