using Ardalis.Specification.EntityFrameworkCore;
using AutoBogus;
using BehaviorTests.Extensions;
using Domain.BoardingCards;
using Domain.TrainCards;
using Domain.TrainCards.Specifications;
using FluentAssertions;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.TrainCards.Queries;

public sealed class TrainCardGetAllTests : TestsBase
{
    private TrainCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task TrainCardGetAll_WhenInputIsGood_ShouldGetAllTrainCards()
    {
        // Arrange
        var trainCards = new AutoFaker<TrainCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, BoardingCardType.Train)
            .RuleFor(x => x.Number, f => f.Random.String2(10))
            .RuleFor(x => x.Departure, f => f.Address.City())
            .RuleFor(x => x.Arrival, f => f.Address.City())
            .RuleFor(x => x.Seat, f => f.Random.String2(10))
            .Generate(5);
        await DbContext.AddRangeAsync(trainCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var actionResult = await Controller.GetAllAsync();

        // Assert
        var resultTrainCards = actionResult.AsOkResult().ToList();
        resultTrainCards.Should().HaveCount(trainCards.Count);
        resultTrainCards.Should().AllSatisfy(resultTrainCard =>
        {
            var trainCard = DbContext.TrainCards.WithSpecification(new TrainCardByIdSpec(resultTrainCard.Id)).Single();
            resultTrainCard.Type.Should().Be(trainCard.Type);
            resultTrainCard.Number.Should().Be(trainCard.Number);
            resultTrainCard.Departure.Should().Be(trainCard.Departure);
            resultTrainCard.Arrival.Should().Be(trainCard.Arrival);
            resultTrainCard.Seat.Should().Be(trainCard.Seat);
        });
    }
}
