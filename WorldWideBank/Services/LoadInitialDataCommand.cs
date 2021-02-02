using System.Data.Entity;
using System.Threading.Tasks;
using NHibernate;
using WorldWideBankSample.Domain;

namespace WorldWideBankSample.Services
{
    public interface ILoadInitialDataCommand
    {
        Task Execute();
    }

    public class LoadInitialDataCommand : ILoadInitialDataCommand
    {
        private readonly ISession _session;

        public LoadInitialDataCommand(ISession session)
        {
            _session = session;
        }

        public async Task Execute()
        {
            var canadianCurrency = new Currency {Code = "CAD", Value = 100, Name = "Canadian Dollar"};
            var mexicanCurrency= new Currency {Code = "MXN", Value = 50, Name = "Mexican Peso"};
            var usaCurrency = new Currency {Code = "USD", Value = 200, Name = "US Dollar"};

            if(await _session.Query<Currency>().SingleOrDefaultAsync(x => x.Code == canadianCurrency.Code) == null)
                await _session.SaveAsync(canadianCurrency);

            if(await _session.Query<Currency>().SingleOrDefaultAsync(x => x.Code == mexicanCurrency.Code) == null)
                await _session.SaveAsync(mexicanCurrency);

            if(await _session.Query<Currency>().SingleOrDefaultAsync(x => x.Code == usaCurrency.Code) == null)
                await _session.SaveAsync(usaCurrency);

            await _session.FlushAsync();
        }
    }
}
