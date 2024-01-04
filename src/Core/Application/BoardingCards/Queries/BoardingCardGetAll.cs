using Application.BoardingCards.Dtos;
using Application.BoardingCards.Mappers;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BoardingCards.Queries;

public static class BoardingCardGetAll
{
    public sealed record Query : IQuery<BoardingCardDto[]>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, BoardingCardDto[]>
    {
        public async Task<BoardingCardDto[]> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.BoardingCards.ToArrayAsync(cancellationToken)).MapToBoardingCardDtos();
    }
}
