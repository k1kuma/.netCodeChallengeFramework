using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using WorldWideBank.Data;

namespace WorldWideBank
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().LoadInitialData().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
