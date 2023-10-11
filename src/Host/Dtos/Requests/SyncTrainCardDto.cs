namespace Host.Dtos.Requests;

public sealed record SyncTrainCardDto(string Number, string Departure, string Arrival, string Seat);
