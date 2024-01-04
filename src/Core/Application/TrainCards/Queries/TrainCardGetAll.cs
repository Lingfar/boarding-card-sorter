using Application.Common.Interfaces;
using Application.TrainCards.Dtos;
using Application.TrainCards.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.TrainCards.Queries;

public static class TrainCardGetAll
{
    public sealed record Query : IQuery<TrainCardDto[]>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, TrainCardDto[]>
    {
        public async Task<TrainCardDto[]> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.TrainCards.ToArrayAsync(cancellationToken)).MapToTrainCardDtos();
    }
}
