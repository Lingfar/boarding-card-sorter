using Application.PlaneCards.Commands;
using BehaviorTests.Extensions;
using Domain.PlaneCards;
using FluentAssertions;
using FluentValidation;
using Host.Controllers;
using Host.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BehaviorTests.PlaneCards.Commands;

public sealed class PlaneCardUpdateTests : TestsBase
{
    private PlaneCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task PlaneCardUpdate_WhenInputIsGood_ShouldUpdatePlaneCard()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        // Act
        var request = new SyncPlaneCardDto("newNumber", "Genève", "Nice", "seat", "gate", "counter");
        var actionResult = await Controller.UpdateAsync(planeCard.Id, request);

        // Assert
        actionResult.IsNoContentResult().Should().BeTrue();

        DbContext.PlaneCards.Should().HaveCount(1);

        var resultPlaneCard = await DbContext.PlaneCards.SingleAsync();
        resultPlaneCard.Number.Should().Be(request.Number);
        resultPlaneCard.Departure.Should().Be(request.Departure);
        resultPlaneCard.Arrival.Should().Be(request.Arrival);
        resultPlaneCard.Seat.Should().Be(request.Seat);
        resultPlaneCard.Gate.Should().Be(request.Gate);
        resultPlaneCard.Counter.Should().Be(request.Counter);
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenNumberIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(planeCard.Id, new SyncPlaneCardDto(string.Empty, "Paris", "London", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Number));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardUpdate.Command.Number)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenDepartureIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(planeCard.Id, new SyncPlaneCardDto("number", string.Empty, "London", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Departure));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardUpdate.Command.Departure)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenArrivalIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(planeCard.Id, new SyncPlaneCardDto("number", "Paris", string.Empty, "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Arrival));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardUpdate.Command.Arrival)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenSeatIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(planeCard.Id, new SyncPlaneCardDto("number", "Paris", "London", string.Empty, "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Seat));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardUpdate.Command.Seat)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenGateIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(planeCard.Id, new SyncPlaneCardDto("number", "Paris", "London", "seat", string.Empty, null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Gate));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardUpdate.Command.Gate)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenNotExists_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var requestId = Guid.NewGuid();
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(requestId, new SyncPlaneCardDto("number", "London", "Genève", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Id));
        error.ErrorMessage.Should().Be($"Plane card {requestId} does not exist.");
    }

    [Fact]
    public async Task PlaneCardUpdate_WhenNumberAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var planeCard = await SeedPlaneCard();

        var otherPlaneCard = new PlaneCard
        {
            Id = Guid.NewGuid(),
            Number = "otherNumber",
            Departure = "Paris",
            Arrival = "London",
            Seat = "seat",
            Gate = "gate",
            Counter = "counter"
        };
        await DbContext.AddAsync(otherPlaneCard);
        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(planeCard.Id, new SyncPlaneCardDto(otherPlaneCard.Number, "London", "Genève", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardUpdate.Command.Number));
        error.ErrorMessage.Should().Be($"Plane card with number {otherPlaneCard.Number} already exists.");
    }

    private async Task<PlaneCard> SeedPlaneCard()
    {
        var planeCard = new PlaneCard
        {
            Id = Guid.NewGuid(),
            Number = "number",
            Departure = "Paris",
            Arrival = "London",
            Seat = "seat",
            Gate = "gate",
            Counter = "counter"
        };
        await DbContext.AddAsync(planeCard);
        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();
        return planeCard;
    }
}
