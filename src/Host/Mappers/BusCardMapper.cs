using Application.BusCards.Commands;
using Host.Dtos.Requests;
using Riok.Mapperly.Abstractions;

namespace Host.Mappers;

[Mapper]
public static partial class BusCardMapper
{
    public static partial BusCardCreate.Command MapToBusCardCreateCommand(this SyncBusCardDto dto);

    [MapperIgnoreTarget(nameof(BusCardUpdate.Command.Id))]
    public static partial BusCardUpdate.Command MapToBusCardUpdateCommand(this SyncBusCardDto dto);
}
