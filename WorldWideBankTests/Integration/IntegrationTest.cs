using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using WorldWideBank;
using WorldWideBank.Services;

namespace WorldWideBankTests.Integration
{
    [TestFixture]
    class IntegrationTest
    {
        // protected WebApplicationFactory<WorldWideBank.Startup> _factory;
        protected TestServer _server;
        protected HttpClient _client;
       

        [SetUp]
        public async Task SetupServer()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            _server = new TestServer(new WebHostBuilder().ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(configPath);
                })
                .UseStartup<Startup>());
            _client = _server.CreateClient();

            var loadInitialDataCommand = Resolve<ILoadInitialDataCommand>();
            await loadInitialDataCommand.Execute();
        }

        [TearDown]
        public async Task TearDownServer()
        {
            _client.Dispose();
            _server.Dispose();
        }

        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");

        public T Resolve<T>()
        {
            return (T) _server.Services.GetService(typeof(T));
        }

    }
}
