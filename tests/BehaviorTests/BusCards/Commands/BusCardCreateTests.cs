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

public sealed class BusCardCreateTests : TestsBase
{
    private BusCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task BusCardCreate_WhenInputIsGood_ShouldCreateBusCard()
    {
        // Arrange

        // Act
        var request = new SyncBusCardDto("number", "Paris", "London", "seat");
        var actionResult = await Controller.CreateAsync(request);

        // Assert
        DbContext.BusCards.Should().HaveCount(1);

        var busCard = await DbContext.BusCards.SingleAsync();
        busCard.Id.Should().Be(actionResult.AsOkResult().Id);
        busCard.Number.Should().Be(request.Number);
        busCard.Departure.Should().Be(request.Departure);
        busCard.Arrival.Should().Be(request.Arrival);
        busCard.Seat.Should().Be(request.Seat);
    }

    [Fact]
    public async Task BusCardCreate_WhenNumberIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.CreateAsync(new SyncBusCardDto(string.Empty, "Paris", "London", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardCreate.Command.Number));
        error.ErrorMessage.Should().Be($"'{nameof(BusCardCreate.Command.Number)}' must not be empty.");
    }

    [Fact]
    public async Task BusCardCreate_WhenDepartureIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.CreateAsync(new SyncBusCardDto("number", string.Empty, "London", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardCreate.Command.Departure));
        error.ErrorMessage.Should().Be($"'{nameof(BusCardCreate.Command.Departure)}' must not be empty.");
    }

    [Fact]
    public async Task BusCardCreate_WhenArrivalIsEmpty_ShouldThrowValidationException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.CreateAsync(new SyncBusCardDto("number", "Paris", string.Empty, null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardCreate.Command.Arrival));
        error.ErrorMessage.Should().Be($"'{nameof(BusCardCreate.Command.Arrival)}' must not be empty.");
    }

    [Fact]
    public async Task BusCardCreate_WhenNumberAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var otherBusCard = new BusCard
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
            => Controller.CreateAsync(new SyncBusCardDto(otherBusCard.Number, "London", "Genève", null)));

        // Assert
        exception.Errors.Should().HaveCount(1);
        var error = exception.Errors.Single();
        error.PropertyName.Should().Be(nameof(BusCardCreate.Command.Number));
        error.ErrorMessage.Should().Be($"Bus card with number {otherBusCard.Number} already exists.");
    }
}
