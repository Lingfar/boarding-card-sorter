using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Common.Middlewares;

public sealed class ValidationBehavior<TCommand, TResponse>(IEnumerable<IValidator<TCommand>> validators)
    : IPipelineBehavior<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            await Task.WhenAll(validators.Select(v => v.ValidateAndThrowAsync(request, cancellationToken)));
        }

        return await next();
    }
}
