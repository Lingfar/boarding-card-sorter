using Application.PlaneCards.Commands;
using Application.PlaneCards.Dtos;
using Domain.PlaneCards;
using Riok.Mapperly.Abstractions;

namespace Application.PlaneCards.Mappers;

[Mapper]
public static partial class PlaneCardMapper
{
    [MapperIgnoreTarget(nameof(PlaneCard.Id))]
    [MapperIgnoreTarget(nameof(PlaneCard.Type))]
    public static partial PlaneCard MapToPlaneCard(this PlaneCardCreate.Command command);

    [MapperIgnoreTarget(nameof(PlaneCard.Id))]
    [MapperIgnoreTarget(nameof(PlaneCard.Type))]
    public static partial void MapToPlaneCard(this PlaneCardUpdate.Command command, PlaneCard entity);

    public static partial PlaneCardDto MapToPlaneCardDto(this PlaneCard entity);
    public static partial IEnumerable<PlaneCardDto> MapToPlaneCardDtos(this List<PlaneCard> entities);
}
