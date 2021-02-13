using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NHibernate;
using WorldWideBank.Data.Core;
using WorldWideBank.Domain;

namespace WorldWideBank.Services
{
    public interface ILoadInitialDataCommand
    {
        Task Execute();
    }

    public class LoadInitialDataCommand : ILoadInitialDataCommand
    {
        private readonly IConfiguration _configuration;

        public LoadInitialDataCommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Execute()
        {
            try
            {
                var databaseSessionFactory = new DatabaseSessionFactory();
                using var session =
                    databaseSessionFactory.Create(_configuration.GetConnectionString("DefaultConnection"));

                using var transaction = session.BeginTransaction();
                transaction.Begin();
                var canadianCurrency = new Currency {Code = "CAD", Value = 100, Name = "Canadian Dollar"};
                var mexicanCurrency = new Currency {Code = "MXN", Value = 50, Name = "Mexican Peso"};
                var usaCurrency = new Currency {Code = "USD", Value = 200, Name = "US Dollar"};

                if (session.Query<Currency>().SingleOrDefault(x => x.Code == canadianCurrency.Code) == null)
                {
                    session.Save(canadianCurrency);
                }

                if (session.Query<Currency>().SingleOrDefault(x => x.Code == mexicanCurrency.Code) == null)
                    session.Save(mexicanCurrency);

                if (session.Query<Currency>().SingleOrDefault(x => x.Code == usaCurrency.Code) == null)
                    session.Save(usaCurrency);

                await session.FlushAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
