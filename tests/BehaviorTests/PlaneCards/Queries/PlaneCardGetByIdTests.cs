using AutoBogus;
using BehaviorTests.Extensions;
using Domain.BoardingCards;
using Domain.PlaneCards;
using FluentAssertions;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.PlaneCards.Queries;

public sealed class PlaneCardGetByIdTests : TestsBase
{
    private PlaneCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task PlaneCardGetById_WhenInputIsGood_ShouldGetPlaneCard()
    {
        // Arrange
        var planeCards = new AutoFaker<PlaneCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, BoardingCardType.Plane)
            .RuleFor(x => x.Number, f => f.Random.String2(10))
            .RuleFor(x => x.Departure, f => f.Address.City())
            .RuleFor(x => x.Arrival, f => f.Address.City())
            .RuleFor(x => x.Seat, f => f.Random.String2(10))
            .RuleFor(x => x.Gate, f => f.Random.String2(10))
            .RuleFor(x => x.Counter, f => f.Random.String2(10))
            .Generate(5);
        await DbContext.AddRangeAsync(planeCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        var planeCard = planeCards[0];

        // Act
        var actionResult = await Controller.GetByIdAsync(planeCard.Id);

        // Assert
        var resultPlaneCard = actionResult.AsOkResult();
        resultPlaneCard.Type.Should().Be(planeCard.Type);
        resultPlaneCard.Number.Should().Be(planeCard.Number);
        resultPlaneCard.Departure.Should().Be(planeCard.Departure);
        resultPlaneCard.Arrival.Should().Be(planeCard.Arrival);
        resultPlaneCard.Seat.Should().Be(planeCard.Seat);
        resultPlaneCard.Gate.Should().Be(planeCard.Gate);
        resultPlaneCard.Counter.Should().Be(planeCard.Counter);
    }

    [Fact]
    public async Task PlaneCardGetById_WhenIdNotExists_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var planeCards = new AutoFaker<PlaneCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, BoardingCardType.Plane)
            .Generate(5);
        await DbContext.AddRangeAsync(planeCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act / Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.GetByIdAsync(Guid.NewGuid()));
    }
}
