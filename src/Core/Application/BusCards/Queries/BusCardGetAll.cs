using Application.BusCards.Dtos;
using Application.BusCards.Mappers;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BusCards.Queries;

public static class BusCardGetAll
{
    public sealed record Query : IQuery<IEnumerable<BusCardDto>>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, IEnumerable<BusCardDto>>
    {
        public async Task<IEnumerable<BusCardDto>> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.BusCards.ToListAsync(cancellationToken)).MapToBusCardDtos();
    }
}
