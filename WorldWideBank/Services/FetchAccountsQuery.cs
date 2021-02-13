using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services
{
    public interface IFetchAccountsQuery
    {
        Task<ICollection<AccountDto>> Fetch();
    }

    public class FetchAccountsQuery : IFetchAccountsQuery
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public FetchAccountsQuery(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }
        public async Task<ICollection<AccountDto>> Fetch()
        {
            var accountDtos = await _session.Query<Account>()
                .Select(x => _mapper.Map<AccountDto>(x))
                .ToListAsync();

            return accountDtos;
        }
    }
}
