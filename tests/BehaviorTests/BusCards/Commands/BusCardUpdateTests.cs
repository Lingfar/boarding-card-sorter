using Application.BusCards.Commands;
using BehaviorTests.Extensions;
using Domain.BusCards;
using FluentAssertions;
using FluentValidation;
using Host.Controllers;
using Host.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BehaviorTests.BusCards.Commands;

public sealed class BusCardUpdateTests : TestsBase
{
    private BusCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task BusCardUpdate_WhenInputIsGood_ShouldUpdateBusCard()
    {
        // Arrange
        var busCard = await SeedBusCard();
        var request = new SyncBusCardDto("newNumber", "Genève", "Nice", null);

        // Act
        var actionResult = await Controller.UpdateAsync(busCard.Id, request);

        // Assert
        actionResult.IsNoContentResult().Should().BeTrue();

        DbContext.BusCards.Should().HaveCount(1);

        var resultBusCard = await DbContext.BusCards.SingleAsync();
        resultBusCard.Number.Should().Be(request.Number);
        resultBusCard.Departure.Should().Be(request.Departure);
        resultBusCard.Arrival.Should().Be(request.Arrival);
        resultBusCard.Seat.Should().Be(request.Seat);
    }

    [Fact]
    public async Task BusCardUpdate_WhenNumberIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var busCard = await SeedBusCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(busCard.Id, new SyncBusCardDto(string.Empty, "Paris", "London", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardUpdate.Command.Number));
        error.ErrorMessage.Should().Be($"'{nameof(BusCardUpdate.Command.Number)}' must not be empty.");
    }

    [Fact]
    public async Task BusCardUpdate_WhenDepartureIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var busCard = await SeedBusCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(busCard.Id, new SyncBusCardDto("number", string.Empty, "London", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardUpdate.Command.Departure));
        error.ErrorMessage.Should().Be($"'{nameof(BusCardUpdate.Command.Departure)}' must not be empty.");
    }

    [Fact]
    public async Task BusCardUpdate_WhenArrivalIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var busCard = await SeedBusCard();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(busCard.Id, new SyncBusCardDto("number", "Paris", string.Empty, null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardUpdate.Command.Arrival));
        error.ErrorMessage.Should().Be($"'{nameof(BusCardUpdate.Command.Arrival)}' must not be empty.");
    }

    [Fact]
    public async Task BusCardUpdate_WhenNotExists_ShouldThrowValidationException()
    {
        // Arrange
        var requestId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(requestId, new SyncBusCardDto("number", "London", "Genève", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardUpdate.Command.Id));
        error.ErrorMessage.Should().Be($"Bus card {requestId} does not exist.");
    }

    [Fact]
    public async Task BusCardUpdate_WhenNumberAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var busCard = await SeedBusCard();

        BusCard otherBusCard = new()
        {
            Id = Guid.NewGuid(),
            Number = "otherNumber",
            Departure = "Paris",
            Arrival = "London",
            Seat = "seat"
        };
        await DbContext.AddAsync(otherBusCard);
        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(()
            => Controller.UpdateAsync(busCard.Id, new SyncBusCardDto(otherBusCard.Number, "London", "Genève", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardUpdate.Command.Number));
        error.ErrorMessage.Should().Be($"Bus card with number {otherBusCard.Number} already exists.");
    }

    private async Task<BusCard> SeedBusCard()
    {
        BusCard busCard = new()
        {
            Id = Guid.NewGuid(),
            Number = "number",
            Departure = "Paris",
            Arrival = "London",
            Seat = "seat"
        };
        await DbContext.AddAsync(busCard);
        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();
        return busCard;
    }
}
