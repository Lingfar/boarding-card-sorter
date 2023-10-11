using Application.BusCards.Commands;
using Application.BusCards.Dtos;
using Domain.BusCards;
using Riok.Mapperly.Abstractions;

namespace Application.BusCards.Mappers;

[Mapper]
public static partial class BusCardMapper
{
    [MapperIgnoreTarget(nameof(BusCard.Id))]
    public static partial BusCard MapToBusCard(this BusCardCreate.Command command);

    [MapperIgnoreTarget(nameof(BusCard.Id))]
    public static partial void MapToBusCard(this BusCardUpdate.Command command, BusCard entity);

    public static partial BusCardDto MapToBusCardDto(this BusCard entity);
    public static partial IEnumerable<BusCardDto> MapToBusCardDtos(this List<BusCard> entities);
}
