using Application.BoardingCards.Commands;
using Application.BusCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.BusCards.Specifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BusCards.Commands;

public static class BusCardUpdate
{
    public sealed record Command : BusCardSync.Command<bool>
    {
        public Guid Id { get; set; }
    }

    public sealed class Validator : BoardingCardSync.Validator<Command, bool>
    {
        public Validator(ReadDbContext readDbContext)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEqual(Guid.Empty)
                .MustAsync((id, cancellationToken) => readDbContext.BusCards.WithSpecification(new BusCardByIdSpec(id)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Bus card {x.Id} does not exist.");

            RuleFor(x => x.Number)
                .MustAsync(async (command, number, cancellationToken)
                    => !await readDbContext.BusCards.WithSpecification(new BusCardUniqueNumberSpec(number, command.Id)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Bus card with number {x.Number} already exists.");
        }
    }

    public sealed class Handler(WriteDbContext dbContext) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var busCard = await dbContext.BusCards.WithSpecification(new BusCardByIdSpec(request.Id)).SingleAsync(cancellationToken);
            request.MapToBusCard(busCard);

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
