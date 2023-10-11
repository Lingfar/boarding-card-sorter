using Application.PlaneCards.Commands;
using Host.Dtos.Requests;
using Riok.Mapperly.Abstractions;

namespace Host.Mappers;

[Mapper]
public static partial class PlaneCardMapper
{
    public static partial PlaneCardCreate.Command MapToPlaneCardCreateCommand(this SyncPlaneCardDto dto);

    [MapperIgnoreTarget(nameof(PlaneCardUpdate.Command.Id))]
    public static partial PlaneCardUpdate.Command MapToPlaneCardUpdateCommand(this SyncPlaneCardDto dto);
}
