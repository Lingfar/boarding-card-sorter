using Application.TrainCards.Commands;
using BehaviorTests.Extensions;
using Domain.TrainCards;
using FluentAssertions;
using FluentValidation;
using Host.Controllers;
using Host.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BehaviorTests.TrainCards.Commands;

public sealed class TrainCardCreateTests : TestsBase
{
    private TrainCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task TrainCardCreate_WhenInputIsGood_ShouldCreateTrainCard()
    {
        // Arrange

        // Act
        var request = new SyncTrainCardDto("number", "Paris", "London", "seat");
        var actionResult = await Controller.CreateAsync(request);

        // Assert
        DbContext.TrainCards.Should().HaveCount(1);

        var trainCard = await DbContext.TrainCards.SingleAsync();
        trainCard.Id.Should().Be(actionResult.AsOkResult().Id);
        trainCard.Number.Should().Be(request.Number);
        trainCard.Departure.Should().Be(request.Departure);
        trainCard.Arrival.Should().Be(request.Arrival);
        trainCard.Seat.Should().Be(request.Seat);
    }

    [Fact]
    public async Task TrainCardCreate_WhenNumberIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncTrainCardDto(string.Empty, "Paris", "London", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardCreate.Command.Number));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardCreate.Command.Number)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardCreate_WhenDepartureIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception =
            await Assert.ThrowsAsync<ValidationException>(() => Controller.CreateAsync(new SyncTrainCardDto("number", string.Empty, "London", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardCreate.Command.Departure));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardCreate.Command.Departure)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardCreate_WhenArrivalIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncTrainCardDto("number", "Paris", string.Empty, "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardCreate.Command.Arrival));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardCreate.Command.Arrival)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardCreate_WhenSeatIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncTrainCardDto("number", "Paris", "London", string.Empty)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardCreate.Command.Seat));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardCreate.Command.Seat)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardCreate_WhenNumberAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var otherTrainCard = new TrainCard
        {
            Id = Guid.NewGuid(),
            Number = "otherNumber",
            Departure = "Paris",
            Arrival = "London",
            Seat = "seat"
        };
        await DbContext.AddAsync(otherTrainCard);
        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncTrainCardDto(otherTrainCard.Number, "London", "Genève", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardCreate.Command.Number));
        error.ErrorMessage.Should().Be($"Train card with number {otherTrainCard.Number} already exists.");
    }
}
