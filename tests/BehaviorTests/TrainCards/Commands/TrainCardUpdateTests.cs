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

public sealed class TrainCardUpdateTests : TestsBase
{
    private TrainCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task TrainCardUpdate_WhenInputIsGood_ShouldUpdateTrainCard()
    {
        // Arrange
        var trainCard = await SeedTrainCard();

        // Act
        var request = new SyncTrainCardDto("newNumber", "Genève", "Nice", "seat");
        var actionResult = await Controller.UpdateAsync(trainCard.Id, request);

        // Assert
        actionResult.IsNoContentResult().Should().BeTrue();

        DbContext.TrainCards.Should().HaveCount(1);

        var resultTrainCard = await DbContext.TrainCards.SingleAsync();
        resultTrainCard.Number.Should().Be(request.Number);
        resultTrainCard.Departure.Should().Be(request.Departure);
        resultTrainCard.Arrival.Should().Be(request.Arrival);
        resultTrainCard.Seat.Should().Be(request.Seat);
    }

    [Fact]
    public async Task TrainCardUpdate_WhenNumberIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var trainCard = await SeedTrainCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(trainCard.Id, new SyncTrainCardDto(string.Empty, "Paris", "London", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardUpdate.Command.Number));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardUpdate.Command.Number)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardUpdate_WhenDepartureIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var trainCard = await SeedTrainCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(trainCard.Id, new SyncTrainCardDto("number", string.Empty, "London", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardUpdate.Command.Departure));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardUpdate.Command.Departure)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardUpdate_WhenArrivalIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var trainCard = await SeedTrainCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(trainCard.Id, new SyncTrainCardDto("number", "Paris", string.Empty, "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardUpdate.Command.Arrival));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardUpdate.Command.Arrival)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardUpdate_WhenSeatIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var trainCard = await SeedTrainCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(trainCard.Id, new SyncTrainCardDto("number", "Paris", "London", string.Empty)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardUpdate.Command.Seat));
        error.ErrorMessage.Should().Be($"'{nameof(TrainCardUpdate.Command.Seat)}' must not be empty.");
    }

    [Fact]
    public async Task TrainCardUpdate_WhenNotExists_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var requestId = Guid.NewGuid();
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(requestId, new SyncTrainCardDto("number", "London", "Genève", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardUpdate.Command.Id));
        error.ErrorMessage.Should().Be($"Train card {requestId} does not exist.");
    }

    [Fact]
    public async Task TrainCardUpdate_WhenNumberAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var trainCard = await SeedTrainCard();

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
            => Controller.UpdateAsync(trainCard.Id, new SyncTrainCardDto(otherTrainCard.Number, "London", "Genève", "seat")));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(TrainCardUpdate.Command.Number));
        error.ErrorMessage.Should().Be($"Train card with number {otherTrainCard.Number} already exists.");
    }

    private async Task<TrainCard> SeedTrainCard()
    {
        var trainCard = new TrainCard
        {
            Id = Guid.NewGuid(),
            Number = "number",
            Departure = "Paris",
            Arrival = "London",
            Seat = "seat"
        };
        await DbContext.AddAsync(trainCard);
        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();
        return trainCard;
    }
}
