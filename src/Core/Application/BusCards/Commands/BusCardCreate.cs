using Application.BoardingCards.Commands;
using Application.BusCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.BusCards.Specifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BusCards.Commands;

public static class BusCardCreate
{
    public sealed record Command : BusCardSync.Command<Guid>;

    public sealed class Validator : BoardingCardSync.Validator<Command, Guid>
    {
        public Validator(ReadDbContext readDbContext)
        {
            RuleFor(x => x.Number)
                .MustAsync(async (_, number, cancellationToken)
                    => !await readDbContext.BusCards.WithSpecification(new BusCardUniqueNumberSpec(number)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Bus card with number {x.Number} already exists.");
        }
    }

    public sealed class Handler(WriteDbContext dbContext) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var busCard = request.MapToBusCard();
            busCard.Id = Guid.NewGuid();

            await dbContext.AddAsync(busCard, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return busCard.Id;
        }
    }
}
