using AutoBogus;
using BehaviorTests.Extensions;
using Domain.TrainCards;
using FluentAssertions;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.TrainCards.Queries;

public sealed class TrainCardGetByIdTests : TestsBase
{
    private TrainCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task TrainCardGetById_WhenInputIsGood_ShouldGetTrainCard()
    {
        // Arrange
        var trainCards = new AutoFaker<TrainCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Number, f => f.Random.String2(10))
            .RuleFor(x => x.Departure, f => f.Address.City())
            .RuleFor(x => x.Arrival, f => f.Address.City())
            .RuleFor(x => x.Seat, f => f.Random.String2(10))
            .Generate(5);
        await DbContext.AddRangeAsync(trainCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        var trainCard = trainCards[0];

        // Act
        var actionResult = await Controller.GetByIdAsync(trainCard.Id);

        // Assert
        var resultTrainCard = actionResult.AsOkResult();
        resultTrainCard.Type.Should().Be(trainCard.Type);
        resultTrainCard.Number.Should().Be(trainCard.Number);
        resultTrainCard.Departure.Should().Be(trainCard.Departure);
        resultTrainCard.Arrival.Should().Be(trainCard.Arrival);
        resultTrainCard.Seat.Should().Be(trainCard.Seat);
    }

    [Fact]
    public async Task TrainCardGetById_WhenIdNotExists_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var trainCards = new AutoFaker<TrainCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .Generate(5);
        await DbContext.AddRangeAsync(trainCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act / Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.GetByIdAsync(Guid.NewGuid()));
    }
}
