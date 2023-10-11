using Application.TrainCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.TrainCards.Specifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.TrainCards.Commands;

public static class TrainCardUpdate
{
    public sealed record Command : TrainCardSync.Command<bool>
    {
        public Guid Id { get; set; }
    }

    public sealed class Validator : TrainCardSync.Validator<Command, bool>
    {
        public Validator(ReadDbContext readDbContext)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEqual(Guid.Empty)
                .MustAsync((id, cancellationToken) => readDbContext.TrainCards.WithSpecification(new TrainCardByIdSpec(id)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Train card {x.Id} does not exist.");

            RuleFor(x => x.Number)
                .MustAsync(async (command, number, cancellationToken)
                    => !await readDbContext.TrainCards.WithSpecification(new TrainCardUniqueNumberSpec(number, command.Id)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Train card with number {x.Number} already exists.");
        }
    }

    public sealed class Handler(WriteDbContext dbContext) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var trainCard = await dbContext.TrainCards.WithSpecification(new TrainCardByIdSpec(request.Id)).SingleAsync(cancellationToken);
            request.MapToTrainCard(trainCard);

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
