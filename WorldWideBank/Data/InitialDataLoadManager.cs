using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldWideBank.Services;

namespace WorldWideBank.Data
{
    public static class InitialDataLoadManager
    {
        public static IHost LoadInitialData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var loadInitialDataCommand = scope.ServiceProvider.GetRequiredService<ILoadInitialDataCommand>();
                loadInitialDataCommand.Execute();
            }
            return host;
        }
    }
}
