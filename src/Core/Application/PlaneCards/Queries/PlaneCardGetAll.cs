using Application.Common.Interfaces;
using Application.PlaneCards.Dtos;
using Application.PlaneCards.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.PlaneCards.Queries;

public static class PlaneCardGetAll
{
    public sealed record Query : IQuery<PlaneCardDto[]>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, PlaneCardDto[]>
    {
        public async Task<PlaneCardDto[]> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.PlaneCards.ToArrayAsync(cancellationToken)).MapToPlaneCardDtos();
    }
}
