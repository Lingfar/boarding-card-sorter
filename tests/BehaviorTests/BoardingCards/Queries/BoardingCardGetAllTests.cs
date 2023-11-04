using Ardalis.Specification.EntityFrameworkCore;
using AutoBogus;
using BehaviorTests.Extensions;
using Domain.BoardingCards;
using Domain.BusCards;
using Domain.BusCards.Specifications;
using Domain.PlaneCards;
using Domain.PlaneCards.Specifications;
using Domain.TrainCards;
using Domain.TrainCards.Specifications;
using FluentAssertions;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.BoardingCards.Queries;

public sealed class BoardingCardGetAllTests : TestsBase
{
    private BoardingCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task BoardingCardGetAll_WhenInputIsGood_ShouldGetAllBoardingCards()
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

        var trainCards = new AutoFaker<TrainCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, BoardingCardType.Train)
            .RuleFor(x => x.Number, f => f.Random.String2(10))
            .RuleFor(x => x.Departure, f => f.Address.City())
            .RuleFor(x => x.Arrival, f => f.Address.City())
            .RuleFor(x => x.Seat, f => f.Random.String2(10))
            .Generate(5);
        await DbContext.AddRangeAsync(trainCards);

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

        // Act
        var actionResult = await Controller.GetAllAsync();

        // Assert
        var boardingCards = actionResult.AsOkResult().ToList();
        boardingCards.Should().HaveCount(busCards.Count + trainCards.Count + planeCards.Count);
        boardingCards.Should()
            .AllSatisfy(resultBoardingCard =>
            {
                BoardingCard boardingCard = resultBoardingCard.Type switch
                {
                    BoardingCardType.Bus => DbContext.BusCards.WithSpecification(new BusCardByIdSpec(resultBoardingCard.Id)).Single(),
                    BoardingCardType.Train => DbContext.TrainCards.WithSpecification(new TrainCardByIdSpec(resultBoardingCard.Id)).Single(),
                    BoardingCardType.Plane => DbContext.PlaneCards.WithSpecification(new PlaneCardByIdSpec(resultBoardingCard.Id)).Single(),
                    _ => throw new Exception("Unknown boarding card type")
                };

                resultBoardingCard.Type.Should().Be(boardingCard.Type);
                resultBoardingCard.Number.Should().Be(boardingCard.Number);
                resultBoardingCard.Departure.Should().Be(boardingCard.Departure);
                resultBoardingCard.Arrival.Should().Be(boardingCard.Arrival);
            });
    }
}
