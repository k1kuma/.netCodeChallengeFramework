using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WorldWideBankSample.Data.Core
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;

        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            try
            {
                await _next(context);
            }
            catch
            {
                unitOfWork.Invalidate();
                throw;
            }

            await unitOfWork.CommitAsync();
        }
    }
}
