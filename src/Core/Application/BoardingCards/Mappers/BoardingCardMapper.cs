using Application.BoardingCards.Dtos;
using Domain.BoardingCards;
using Riok.Mapperly.Abstractions;

namespace Application.BoardingCards.Mappers;

[Mapper]
public static partial class BoardingCardMapper
{
    public static partial IEnumerable<BoardingCardDto> MapToBoardingCardDtos(this List<BoardingCard> entities);
}
