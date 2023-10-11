using Application.TrainCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.TrainCards.Specifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.TrainCards.Commands;

public static class TrainCardCreate
{
    public sealed record Command : TrainCardSync.Command<Guid>;

    public sealed class Validator : TrainCardSync.Validator<Command, Guid>
    {
        public Validator(ReadDbContext readDbContext)
        {
            RuleFor(x => x.Number)
                .MustAsync(async (_, number, cancellationToken)
                    => !await readDbContext.TrainCards.WithSpecification(new TrainCardUniqueNumberSpec(number)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Train card with number {x.Number} already exists.");
        }
    }

    public sealed class Handler(WriteDbContext dbContext) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var trainCard = request.MapToTrainCard();
            trainCard.Id = Guid.NewGuid();

            await dbContext.AddAsync(trainCard, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return trainCard.Id;
        }
    }
}
