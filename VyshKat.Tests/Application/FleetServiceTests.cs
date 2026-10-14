using FluentAssertions;
using Moq;
using VyshKat.Application;
using VyshKat.Domain.Abstractions;
using VyshKat.Domain.Gear;
using VyshKat.Domain.Services;
using VyshKat.Domain.Vehicles;
using Xunit;

namespace VyshKat.Tests.Application;

public class FleetServiceTests
{
    private readonly Mock<IFleetRepository> _repository = new();
    private readonly Mock<IServiceCenter> _serviceCenter = new();
    private readonly FleetService _sut;

    public FleetServiceTests()
    {
        _sut = new FleetService(_repository.Object, _serviceCenter.Object);
    }

    [Fact]
    public void Constructor_Throws_WhenRepositoryIsNull()
    {
        var act = () => new FleetService(null!, _serviceCenter.Object);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Throws_WhenServiceCenterIsNull()
    {
        var act = () => new FleetService(_repository.Object, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryAccept_Throws_WhenItemIsNull()
    {
        var act = () => _sut.TryAccept(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryAccept_ReturnsFalse_WhenServiceCenterRejects()
    {
        var scooter = new Scooter("SC-1", "Xiaomi", 7);
        _serviceCenter.Setup(s => s.Approve(scooter)).Returns(false);

        var result = _sut.TryAccept(scooter);

        result.Should().BeFalse();
        _repository.Verify(r => r.Add(It.IsAny<IInventoryItem>()), Times.Never);
    }

    [Fact]
    public void TryAccept_ReturnsFalse_WhenInventoryNumberAlreadyExists()
    {
        var scooter = new Scooter("SC-1", "Xiaomi", 7);
        _serviceCenter.Setup(s => s.Approve(scooter)).Returns(true);
        _repository.Setup(r => r.GetByInventoryNumber("SC-1")).Returns(scooter);

        var result = _sut.TryAccept(scooter);

        result.Should().BeFalse();
        _repository.Verify(r => r.Add(It.IsAny<IInventoryItem>()), Times.Never);
    }

    [Fact]
    public void TryAccept_ReturnsTrue_AndAddsToRepository_ForValidNewItem()
    {
        var scooter = new Scooter("SC-1", "Xiaomi", 7);
        _serviceCenter.Setup(s => s.Approve(scooter)).Returns(true);
        _repository.Setup(r => r.GetByInventoryNumber("SC-1")).Returns((IInventoryItem?)null);

        var result = _sut.TryAccept(scooter);

        result.Should().BeTrue();
        _repository.Verify(r => r.Add(scooter), Times.Once);
    }

    [Fact]
    public void TryAccept_DoesNotCallRepository_WhenServiceCenterRejects()
    {
        var scooter = new Scooter("SC-1", "Xiaomi", 7);
        _serviceCenter.Setup(s => s.Approve(scooter)).Returns(false);

        _sut.TryAccept(scooter);

        _repository.Verify(r => r.GetByInventoryNumber(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetItem_ReturnsWhateverRepositoryReturns()
    {
        var scooter = new Scooter("SC-1", "Xiaomi", 7);
        _repository.Setup(r => r.GetByInventoryNumber("SC-1")).Returns(scooter);

        var result = _sut.GetItem("SC-1");
        Assert.Same(scooter, result);
    }

    [Fact]
    public void GetAllItems_ReturnsWhateverRepositoryReturns()
    {
        var items = new IInventoryItem[]
        {
            new Scooter("SC-1", "Xiaomi", 7),
            new Helmet("H-1", "Bell")
        };
        _repository.Setup(r => r.GetAll()).Returns(items);

        var result = _sut.GetAllItems();

        result.Should().BeEquivalentTo(items);
    }
}