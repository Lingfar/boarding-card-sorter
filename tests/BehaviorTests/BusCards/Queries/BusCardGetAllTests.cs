using Ardalis.Specification.EntityFrameworkCore;
using AutoBogus;
using BehaviorTests.Extensions;
using Domain.BusCards;
using Domain.BusCards.Specifications;
using FluentAssertions;
using Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BehaviorTests.BusCards.Queries;

public sealed class BusCardGetAllTests : TestsBase
{
    private BusCardController Controller => new(Mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task BusCardGetAll_WhenInputIsGood_ShouldGetAllBusCards()
    {
        // Arrange
        var busCards = new AutoFaker<BusCard>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Number, f => f.Random.String2(10))
            .RuleFor(x => x.Departure, f => f.Address.City())
            .RuleFor(x => x.Arrival, f => f.Address.City())
            .RuleFor(x => x.Seat, f => f.Random.String2(10))
            .Generate(5);
        await DbContext.AddRangeAsync(busCards);

        await DbContext.SaveChangesAsync();
        DbContext.DetachAllEntries();

        // Act
        var actionResult = await Controller.GetAllAsync();

        // Assert
        var resultBusCards = actionResult.AsOkResult().ToList();
        resultBusCards.Should().HaveCount(busCards.Count);
        resultBusCards.Should()
            .AllSatisfy(resultBusCard =>
            {
                var busCard = DbContext.BusCards.WithSpecification(new BusCardByIdSpec(resultBusCard.Id)).Single();
                resultBusCard.Type.Should().Be(busCard.Type);
                resultBusCard.Number.Should().Be(busCard.Number);
                resultBusCard.Departure.Should().Be(busCard.Departure);
                resultBusCard.Arrival.Should().Be(busCard.Arrival);
                resultBusCard.Seat.Should().Be(busCard.Seat);
            });
    }
}
