using Application.TrainCards.Commands;
using Host.Dtos.Requests;
using Riok.Mapperly.Abstractions;

namespace Host.Mappers;

[Mapper]
public static partial class TrainCardMapper
{
    public static partial TrainCardCreate.Command MapToTrainCardCreateCommand(this SyncTrainCardDto dto);

    [MapperIgnoreTarget(nameof(TrainCardUpdate.Command.Id))]
    public static partial TrainCardUpdate.Command MapToTrainCardUpdateCommand(this SyncTrainCardDto dto);
}
