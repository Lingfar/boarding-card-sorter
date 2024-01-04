using BehaviorTests.Extensions;
using Domain.BoardingCards;
using Domain.BusCards;
using Domain.PlaneCards;
using Domain.TrainCards;
using FluentAssertions;
using FluentValidation;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.BoardingCards.Commands;

public sealed class BoardingCardOrderTests : TestsBase
{
    private BoardingCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task BoardingCardOrder_WhenInputIsGood_ShouldOrderBoardingCardsAndReturnJourney()
    {
        // Arrange
        // Madrid -> Barcelona -> Gerona Airport -> Stockholm -> New York JFK
        PlaneCard card1 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK455",
            Departure = "Gerona Airport",
            Arrival = "Stockholm",
            Seat = "3A",
            Gate = "45B",
            Counter = "344"
        };
        TrainCard card2 = new()
        {
            Id = Guid.NewGuid(),
            Number = "78A",
            Departure = "Madrid",
            Arrival = "Barcelona",
            Seat = "45B"
        };
        BusCard card3 = new()
        {
            Id = Guid.NewGuid(),
            Number = "B1337",
            Departure = "Barcelona",
            Arrival = "Gerona Airport",
            Seat = null
        };
        PlaneCard card4 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK22",
            Departure = "Stockholm",
            Arrival = "New York JFK",
            Seat = "7B",
            Gate = "22",
            Counter = null
        };
        await DbContext.AddRangeAsync(card1, card2, card3, card4);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var actionResult = await Controller.OrderAndGetJourneyAsync();

        // Assert
        actionResult.AsOkResult().Should().Be(
            """
            Take train 78A from Madrid to Barcelona. Sit in seat 45B.
            Take bus B1337 from Barcelona to Gerona Airport. No seat assignment.
            From Gerona Airport, take flight SK455 to Stockholm. Gate 45B, seat 3A. Baggage drop at ticket counter 344.
            From Stockholm, take flight SK22 to New York JFK. Gate 22, seat 7B. Baggage will be automatically transferred from your last leg.
            You have arrived at your final destination.
            """);
    }

    [Fact]
    public async Task BoardingCardOrder_WhenNoStart_ShouldThrowValidationException()
    {
        // Arrange
        // Madrid -> Barcelona -> Gerona Airport -> Stockholm -> Madrid
        PlaneCard card1 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK455",
            Departure = "Gerona Airport",
            Arrival = "Stockholm",
            Seat = "3A",
            Gate = "45B",
            Counter = "344"
        };
        TrainCard card2 = new()
        {
            Id = Guid.NewGuid(),
            Number = "78A",
            Departure = "Madrid",
            Arrival = "Barcelona",
            Seat = "45B"
        };
        BusCard card3 = new()
        {
            Id = Guid.NewGuid(),
            Number = "B1337",
            Departure = "Barcelona",
            Arrival = "Gerona Airport",
            Seat = null
        };
        PlaneCard card4 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK22",
            Departure = "Stockholm",
            Arrival = "Madrid",
            Seat = "7B",
            Gate = "22",
            Counter = null
        };
        await DbContext.AddRangeAsync(card1, card2, card3, card4);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.OrderAndGetJourneyAsync());

        // Assert
        exception.Message.Should().Be("Cannot find the start of the journey.");
    }

    [Fact]
    public async Task BoardingCardOrder_WhenSameDeparture_ShouldThrowValidationException()
    {
        // Arrange
        // Madrid -> Barcelona -> Gerona Airport -> Stockholm
        // Madrid -> New York JFK
        PlaneCard card1 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK455",
            Departure = "Gerona Airport",
            Arrival = "Stockholm",
            Seat = "3A",
            Gate = "45B",
            Counter = "344"
        };
        TrainCard card2 = new()
        {
            Id = Guid.NewGuid(),
            Number = "78A",
            Departure = "Madrid",
            Arrival = "Barcelona",
            Seat = "45B"
        };
        BusCard card3 = new()
        {
            Id = Guid.NewGuid(),
            Number = "B1337",
            Departure = "Barcelona",
            Arrival = "Gerona Airport",
            Seat = null
        };
        PlaneCard card4 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK22",
            Departure = "Madrid",
            Arrival = "New York JFK",
            Seat = "7B",
            Gate = "22",
            Counter = null
        };
        await DbContext.AddRangeAsync(card1, card2, card3, card4);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.OrderAndGetJourneyAsync());

        // Assert
        exception.Message.Should().Be($"Duplicate '{nameof(BoardingCard.Departure)}' location found: Madrid.");
    }

    [Fact]
    public async Task BoardingCardOrder_WhenSameArrival_ShouldThrowValidationException()
    {
        // Arrange
        // Madrid -> Barcelona -> Gerona Airport -> Stockholm -> Barcelona
        PlaneCard card1 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK455",
            Departure = "Gerona Airport",
            Arrival = "Stockholm",
            Seat = "3A",
            Gate = "45B",
            Counter = "344"
        };
        TrainCard card2 = new()
        {
            Id = Guid.NewGuid(),
            Number = "78A",
            Departure = "Madrid",
            Arrival = "Barcelona",
            Seat = "45B"
        };
        BusCard card3 = new()
        {
            Id = Guid.NewGuid(),
            Number = "B1337",
            Departure = "Barcelona",
            Arrival = "Gerona Airport",
            Seat = null
        };
        PlaneCard card4 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK22",
            Departure = "Stockholm",
            Arrival = "Barcelona",
            Seat = "7B",
            Gate = "22",
            Counter = null
        };
        await DbContext.AddRangeAsync(card1, card2, card3, card4);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.OrderAndGetJourneyAsync());

        // Assert
        exception.Message.Should().Be($"Duplicate '{nameof(BoardingCard.Arrival)}' location found: Barcelona.");
    }

    [Fact]
    public async Task BoardingCardOrder_WhenHasGap_ShouldThrowValidationException()
    {
        // Arrange
        // Madrid -> Barcelona -> Gerona Airport -> Paris -> ? -> Stockholm -> New York JFK
        PlaneCard card1 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK455",
            Departure = "Gerona Airport",
            Arrival = "Paris",
            Seat = "3A",
            Gate = "45B",
            Counter = "344"
        };
        TrainCard card2 = new()
        {
            Id = Guid.NewGuid(),
            Number = "78A",
            Departure = "Madrid",
            Arrival = "Barcelona",
            Seat = "45B"
        };
        BusCard card3 = new()
        {
            Id = Guid.NewGuid(),
            Number = "B1337",
            Departure = "Barcelona",
            Arrival = "Gerona Airport",
            Seat = null
        };
        PlaneCard card4 = new()
        {
            Id = Guid.NewGuid(),
            Number = "SK22",
            Departure = "Stockholm",
            Arrival = "New York JFK",
            Seat = "7B",
            Gate = "22",
            Counter = null
        };
        await DbContext.AddRangeAsync(card1, card2, card3, card4);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.OrderAndGetJourneyAsync());

        // Assert
        exception.Message.Should().Be("There are gaps in the journey.");
    }
}
