namespace Host.Dtos.Requests;

public sealed record SyncPlaneCardDto(string Number, string Departure, string Arrival, string Seat, string Gate, string? Counter);
