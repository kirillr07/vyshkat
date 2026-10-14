using FluentAssertions;
using Moq;
using VyshKat.Domain.Abstractions;
using VyshKat.Domain.Gear;
using VyshKat.Domain.Services;
using VyshKat.Domain.Vehicles;
using Xunit;

namespace VyshKat.Tests.Domain;

public class ServiceCenterTests
{
    private readonly ServiceCenter _sut = new();

    [Fact]
    public void Approve_ReturnsFalse_WhenItemIsNull()
    {
        _sut.Approve(null!).Should().BeFalse();
    }

    [Fact]
    public void Approve_ReturnsTrue_ForValidScooter()
    {
        var scooter = new Scooter("SC-1", "Xiaomi", 7);

        _sut.Approve(scooter).Should().BeTrue();
    }

    [Fact]
    public void Approve_ReturnsTrue_ForHelmet_WithoutOptionalInterfaces()
    {
        var helmet = new Helmet("H-1", "Bell");

        _sut.Approve(helmet).Should().BeTrue();
    }

    [Fact]
    public void Approve_ReturnsTrue_ForChargingCabinet_WithZeroEnergy()
    {
        var cabinet = new ChargingCabinet("CC-1", "Cabinet", 0m);

        _sut.Approve(cabinet).Should().BeTrue();
    }

    [Fact]
    public void Approve_ReturnsFalse_ForChargingCabinet_WithNegativeEnergy()
    {
        var cabinet = new ChargingCabinet("CC-1", "Cabinet", -1m);

        _sut.Approve(cabinet).Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Approve_ReturnsFalse_ForInvalidNoviceRating(int rating)
    {
        var scooter = new Scooter("SC-1", "Xiaomi", rating);

        _sut.Approve(scooter).Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(10)]
    public void Approve_ReturnsTrue_ForValidNoviceRating(int rating)
    {
        var scooter = new Scooter("SC-1", "Xiaomi", rating);

        _sut.Approve(scooter).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Approve_ReturnsFalse_WhenInventoryNumberIsBlank(string inv)
    {
        var item = new Mock<IInventoryItem>();
        item.Setup(i => i.InventoryNumber).Returns(inv);
        item.Setup(i => i.Name).Returns("Valid");

        _sut.Approve(item.Object).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Approve_ReturnsFalse_WhenNameIsBlank(string name)
    {
        var item = new Mock<IInventoryItem>();
        item.Setup(i => i.InventoryNumber).Returns("INV-1");
        item.Setup(i => i.Name).Returns(name);

        _sut.Approve(item.Object).Should().BeFalse();
    }
}