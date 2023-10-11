using Ardalis.Specification;

namespace Domain.PlaneCards.Specifications;

public sealed class PlaneCardUniqueNumberSpec : Specification<PlaneCard>
{
    public PlaneCardUniqueNumberSpec(string number)
    {
        Query.Where(card => card.Number == number);
    }

    public PlaneCardUniqueNumberSpec(string number, Guid id) : this(number)
    {
        Query.Where(card => card.Id != id);
    }
}
