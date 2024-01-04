namespace Host.Dtos.Requests;

public sealed record SyncBusCardDto
{
    public string Number { get; set; } = string.Empty;
    public string Departure { get; set; } = string.Empty;
    public string Arrival { get; set; } = string.Empty;
    public string? Seat { get; set; }

    public SyncBusCardDto()
    {
    }

    public SyncBusCardDto(string number, string departure, string arrival, string? seat)
    {
        Number = number;
        Departure = departure;
        Arrival = arrival;
        Seat = seat;
    }
}
