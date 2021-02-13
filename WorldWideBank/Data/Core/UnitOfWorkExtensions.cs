using Microsoft.AspNetCore.Builder;

namespace WorldWideBank.Data.Core
{
    public static class UnitOfWorkExtensions
    {
        public static IApplicationBuilder UseUnitOfWork(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnitOfWorkMiddleware>();
        }
    }
}
