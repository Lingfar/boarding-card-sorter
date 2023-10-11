using Application.Common.Interfaces;
using Application.PlaneCards.Dtos;
using Application.PlaneCards.Mappers;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.PlaneCards.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.PlaneCards.Queries;

public static class PlaneCardGetById
{
    public sealed record Query(Guid Id) : IQuery<PlaneCardDto>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, PlaneCardDto>
    {
        public async Task<PlaneCardDto> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.PlaneCards.WithSpecification(new PlaneCardByIdSpec(request.Id)).FirstOrDefaultAsync(cancellationToken))?.MapToPlaneCardDto()
               ?? throw new KeyNotFoundException();
    }
}
