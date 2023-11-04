using Application.TrainCards.Commands;
using Application.TrainCards.Dtos;
using Domain.TrainCards;
using Riok.Mapperly.Abstractions;

namespace Application.TrainCards.Mappers;

[Mapper]
public static partial class TrainCardMapper
{
    [MapperIgnoreTarget(nameof(TrainCard.Id))]
    [MapperIgnoreTarget(nameof(TrainCard.Type))]
    public static partial TrainCard MapToTrainCard(this TrainCardCreate.Command command);

    [MapperIgnoreTarget(nameof(TrainCard.Id))]
    [MapperIgnoreTarget(nameof(TrainCard.Type))]
    public static partial void MapToTrainCard(this TrainCardUpdate.Command command, TrainCard entity);

    public static partial TrainCardDto MapToTrainCardDto(this TrainCard entity);
    public static partial IEnumerable<TrainCardDto> MapToTrainCardDtos(this List<TrainCard> entities);
}
