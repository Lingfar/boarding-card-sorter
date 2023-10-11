using Application.BoardingCards.Commands;
using FluentValidation;

namespace Application.PlaneCards.Commands;

public static class PlaneCardSync
{
    public abstract record Command<TResponse> : BoardingCardSync.Command<TResponse>
    {
        public string Seat { get; set; } = null!;
        public string Gate { get; set; } = null!;
        public string? Counter { get; set; }
    }

    public abstract class Validator<TCommand, TResponse> : BoardingCardSync.Validator<TCommand, TResponse>
        where TCommand : Command<TResponse>
    {
        protected Validator()
        {
            RuleFor(x => x.Seat)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Gate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();
        }
    }
}
