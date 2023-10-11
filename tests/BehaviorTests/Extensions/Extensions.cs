using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BehaviorTests.Extensions;

public static class Extensions
{
    public static void DetachAllEntries(this DbContext dbContext) => dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Detached);

    public static T AsOkResult<T>(this ActionResult<T> result) => (T)((OkObjectResult)result.Result!).Value!;

    public static bool IsNoContentResult(this ActionResult result) => result is NoContentResult;
}
