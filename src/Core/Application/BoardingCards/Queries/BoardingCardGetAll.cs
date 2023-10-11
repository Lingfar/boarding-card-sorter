using Application.BoardingCards.Dtos;
using Application.BoardingCards.Mappers;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BoardingCards.Queries;

public static class BoardingCardGetAll
{
    public sealed record Query : IQuery<IEnumerable<BoardingCardDto>>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, IEnumerable<BoardingCardDto>>
    {
        public async Task<IEnumerable<BoardingCardDto>> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.BoardingCards.ToListAsync(cancellationToken)).MapToBoardingCardDtos();
    }
}
