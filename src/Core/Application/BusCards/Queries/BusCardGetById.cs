using Application.BusCards.Dtos;
using Application.BusCards.Mappers;
using Application.Common.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.BusCards.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Application.BusCards.Queries;

public static class BusCardGetById
{
    public sealed record Query(Guid Id) : IQuery<BusCardDto>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Query, BusCardDto>
    {
        public async Task<BusCardDto> Handle(Query request, CancellationToken cancellationToken)
            => (await readDbContext.BusCards.WithSpecification(new BusCardByIdSpec(request.Id)).FirstOrDefaultAsync(cancellationToken))?.MapToBusCardDto()
               ?? throw new KeyNotFoundException();
    }
}
