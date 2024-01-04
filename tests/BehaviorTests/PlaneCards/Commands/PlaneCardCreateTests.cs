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

public sealed class PlaneCardCreateTests : TestsBase
{
    private PlaneCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task PlaneCardCreate_WhenInputIsGood_ShouldCreatePlaneCard()
    {
        // Arrange
        var request = new SyncPlaneCardDto("number", "Paris", "London", "seat", "gate", "counter");

        // Act
        var actionResult = await Controller.CreateAsync(request);

        // Assert
        DbContext.PlaneCards.Should().HaveCount(1);

        var planeCard = await DbContext.PlaneCards.SingleAsync();
        planeCard.Id.Should().Be(actionResult.AsOkResult().Id);
        planeCard.Number.Should().Be(request.Number);
        planeCard.Departure.Should().Be(request.Departure);
        planeCard.Arrival.Should().Be(request.Arrival);
        planeCard.Seat.Should().Be(request.Seat);
        planeCard.Gate.Should().Be(request.Gate);
        planeCard.Counter.Should().Be(request.Counter);
    }

    [Fact]
    public async Task PlaneCardCreate_WhenNumberIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncPlaneCardDto(string.Empty, "Paris", "London", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardCreate.Command.Number));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardCreate.Command.Number)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardCreate_WhenDepartureIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncPlaneCardDto("number", string.Empty, "London", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardCreate.Command.Departure));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardCreate.Command.Departure)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardCreate_WhenArrivalIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncPlaneCardDto("number", "Paris", string.Empty, "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardCreate.Command.Arrival));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardCreate.Command.Arrival)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardCreate_WhenSeatIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncPlaneCardDto("number", "Paris", "London", string.Empty, "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardCreate.Command.Seat));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardCreate.Command.Seat)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardCreate_WhenGateIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.CreateAsync(new SyncPlaneCardDto("number", "Paris", "London", "seat", string.Empty, null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardCreate.Command.Gate));
        error.ErrorMessage.Should().Be($"'{nameof(PlaneCardCreate.Command.Gate)}' must not be empty.");
    }

    [Fact]
    public async Task PlaneCardCreate_WhenNumberAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        PlaneCard otherPlaneCard = new()
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
            => Controller.CreateAsync(new SyncPlaneCardDto(otherPlaneCard.Number, "London", "Genève", "seat", "gate", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(PlaneCardCreate.Command.Number));
        error.ErrorMessage.Should().Be($"Plane card with number {otherPlaneCard.Number} already exists.");
    }
}
