using Application.BoardingCards.Commands;

namespace Application.BusCards.Commands;

public static class BusCardSync
{
    public abstract record Command<TResponse> : BoardingCardSync.Command<TResponse>
    {
        public string? Seat { get; set; }
    }
}
