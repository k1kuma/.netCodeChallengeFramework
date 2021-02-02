using Microsoft.AspNetCore.Builder;

namespace WorldWideBankSample.Data.Core
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
