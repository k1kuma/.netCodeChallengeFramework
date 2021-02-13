using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services
{
    public interface IFetchAccountQuery
    {
        Task<AccountDto> Fetch(int accountNumber);
    }

    public class FetchAccountQuery : IFetchAccountQuery
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public FetchAccountQuery(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }
        public async Task<AccountDto> Fetch(int accountNumber)
        {
            var accountDto = await _session.Query<Account>().Where(x => x.AccountNumber == accountNumber)
                .Select(x => _mapper.Map<AccountDto>(x))
                .SingleOrDefaultAsync();

            return accountDto;
        }
    }
}
