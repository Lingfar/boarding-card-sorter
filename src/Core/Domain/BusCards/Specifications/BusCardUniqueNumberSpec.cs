using Ardalis.Specification;

namespace Domain.BusCards.Specifications;

public sealed class BusCardUniqueNumberSpec : Specification<BusCard>
{
    public BusCardUniqueNumberSpec(string number)
    {
        Query.Where(card => card.Number == number);
    }

    public BusCardUniqueNumberSpec(string number, Guid id) : this(number)
    {
        Query.Where(card => card.Id != id);
    }
}
