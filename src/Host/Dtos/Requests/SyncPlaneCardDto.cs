namespace Host.Dtos.Requests;

public sealed record SyncPlaneCardDto
{
    public string Number { get; set; } = string.Empty;
    public string Departure { get; set; } = string.Empty;
    public string Arrival { get; set; } = string.Empty;
    public string Seat { get; set; } = string.Empty;
    public string Gate { get; set; } = string.Empty;
    public string? Counter { get; set; }

    public SyncPlaneCardDto()
    {
    }

    public SyncPlaneCardDto(string number, string departure, string arrival, string seat, string gate, string? counter)
    {
        Number = number;
        Departure = departure;
        Arrival = arrival;
        Seat = seat;
        Gate = gate;
        Counter = counter;
    }
}
