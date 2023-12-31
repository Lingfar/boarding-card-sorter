using Application.BusCards.Commands;
using Application.BusCards.Dtos;
using Domain.BusCards;
using Riok.Mapperly.Abstractions;

namespace Application.BusCards.Mappers;

[Mapper]
public static partial class BusCardMapper
{
    [MapperIgnoreTarget(nameof(BusCard.Id))]
    [MapperIgnoreTarget(nameof(BusCard.Type))]
    public static partial BusCard MapToBusCard(this BusCardCreate.Command command);

    [MapperIgnoreTarget(nameof(BusCard.Id))]
    [MapperIgnoreTarget(nameof(BusCard.Type))]
    public static partial void MapToBusCard(this BusCardUpdate.Command command, BusCard entity);

    public static partial BusCardDto MapToBusCardDto(this BusCard entity);
    public static partial BusCardDto[] MapToBusCardDtos(this BusCard[] entities);
}
