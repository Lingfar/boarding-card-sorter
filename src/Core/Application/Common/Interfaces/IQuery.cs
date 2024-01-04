using MediatR;

namespace Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>;
