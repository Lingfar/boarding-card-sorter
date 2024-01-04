using Application.BusCards.Dtos;
using Application.BusCards.Mappers;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BusCards.Queries;

public static class BusCardGetAll
{
    public sealed record Query : IQuery<BusCardDto[]>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, BusCardDto[]>
    {
        public async Task<BusCardDto[]> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.BusCards.ToArrayAsync(cancellationToken)).MapToBusCardDtos();
    }
}
