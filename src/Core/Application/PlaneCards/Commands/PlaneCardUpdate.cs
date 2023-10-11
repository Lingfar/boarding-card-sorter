using Application.PlaneCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.PlaneCards.Specifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.PlaneCards.Commands;

public static class PlaneCardUpdate
{
    public sealed record Command : PlaneCardSync.Command<bool>
    {
        public Guid Id { get; set; }
    }

    public sealed class Validator : PlaneCardSync.Validator<Command, bool>
    {
        public Validator(ReadDbContext readDbContext)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEqual(Guid.Empty)
                .MustAsync((id, cancellationToken) => readDbContext.PlaneCards.WithSpecification(new PlaneCardByIdSpec(id)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Plane card {x.Id} does not exist.");

            RuleFor(x => x.Number)
                .MustAsync(async (command, number, cancellationToken)
                    => !await readDbContext.PlaneCards.WithSpecification(new PlaneCardUniqueNumberSpec(number, command.Id)).AnyAsync(cancellationToken))
                .WithMessage(x => $"Plane card with number {x.Number} already exists.");
        }
    }

    public sealed class Handler(WriteDbContext dbContext) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var planeCard = await dbContext.PlaneCards.WithSpecification(new PlaneCardByIdSpec(request.Id)).SingleAsync(cancellationToken);
            request.MapToPlaneCard(planeCard);

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
