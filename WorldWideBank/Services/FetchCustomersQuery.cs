using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services
{
    public interface IFetchCustomersQuery
    {
        Task<ICollection<CustomerDto>> Fetch();
    }

    public class FetchCustomersQuery : IFetchCustomersQuery
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public FetchCustomersQuery(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task<ICollection<CustomerDto>> Fetch()
        {
            var customers = await _session.Query<Customer>().ToListAsync();

            return _mapper.Map<ICollection<CustomerDto>>(customers);
        }
    }
}
