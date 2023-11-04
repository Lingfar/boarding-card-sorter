using AutoBogus;
using BehaviorTests.Extensions;
using Domain.BoardingCards;
using Domain.BusCards;
using FluentAssertions;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.BusCards.Queries;

public sealed class BusCardGetByIdTests : TestsBase
{
    private BusCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task BusCardGetById_WhenInputIsGood_ShouldGetBusCard()
    {
        // Arrange
        var busCards = new AutoFaker<BusCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, BoardingCardType.Bus)
            .RuleFor(x => x.Number, f => f.Random.String2(10))
            .RuleFor(x => x.Departure, f => f.Address.City())
            .RuleFor(x => x.Arrival, f => f.Address.City())
            .RuleFor(x => x.Seat, f => f.Random.String2(10))
            .Generate(5);
        await DbContext.AddRangeAsync(busCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        var busCard = busCards[0];

        // Act
        var actionResult = await Controller.GetByIdAsync(busCard.Id);

        // Assert
        var resultBusCard = actionResult.AsOkResult();
        resultBusCard.Type.Should().Be(busCard.Type);
        resultBusCard.Number.Should().Be(busCard.Number);
        resultBusCard.Departure.Should().Be(busCard.Departure);
        resultBusCard.Arrival.Should().Be(busCard.Arrival);
        resultBusCard.Seat.Should().Be(busCard.Seat);
    }

    [Fact]
    public async Task BusCardGetById_WhenIdNotExists_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var busCards = new AutoFaker<BusCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, BoardingCardType.Bus)
            .Generate(5);
        await DbContext.AddRangeAsync(busCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act / Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.GetByIdAsync(Guid.NewGuid()));
    }
}
