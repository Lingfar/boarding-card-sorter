using Application.Common.Interfaces;
using Application.TrainCards.Dtos;
using Application.TrainCards.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.TrainCards.Queries;

public static class TrainCardGetAll
{
    public sealed record Query : IQuery<IEnumerable<TrainCardDto>>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, IEnumerable<TrainCardDto>>
    {
        public async Task<IEnumerable<TrainCardDto>> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.TrainCards.ToListAsync(cancellationToken)).MapToTrainCardDtos();
    }
}
