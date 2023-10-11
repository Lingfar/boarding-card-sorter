using Application.Common.Interfaces;
using Application.TrainCards.Dtos;
using Application.TrainCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.TrainCards.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.TrainCards.Queries;

public static class TrainCardGetById
{
    public sealed record Query(Guid Id) : IQuery<TrainCardDto>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, TrainCardDto>
    {
        public async Task<TrainCardDto> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.TrainCards.WithSpecification(new TrainCardByIdSpec(request.Id)).FirstOrDefaultAsync(cancellationToken))?.MapToTrainCardDto()
               ?? throw new KeyNotFoundException();
    }
}
