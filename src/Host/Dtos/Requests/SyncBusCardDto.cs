namespace Host.Dtos.Requests;

public sealed record SyncBusCardDto(string Number, string Departure, string Arrival, string? Seat);
