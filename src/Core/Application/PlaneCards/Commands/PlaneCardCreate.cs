using Application.PlaneCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.PlaneCards.Specifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.PlaneCards.Commands;

public static class PlaneCardCreate
{
    public sealed record Command : PlaneCardSync.Command<Guid>;

    public sealed class Validator : PlaneCardSync.Validator<Command, Guid>
    {
        public Validator(ReadDbContext readDbContext)
        {
            RuleFor(x => x.Number)
                .MustAsync(async (_, number, cancellationToken)
                    => !await readDbContext.PlaneCards.WithSpecification(new PlaneCardUniqueNumberSpec(number)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Plane card with number {x.Number} already exists.");
        }
    }

    public sealed class Handler(WriteDbContext dbContext) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var planeCard = request.MapToPlaneCard();
            planeCard.Id = Guid.NewGuid();

            await dbContext.AddAsync(planeCard, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return planeCard.Id;
        }
    }
}
