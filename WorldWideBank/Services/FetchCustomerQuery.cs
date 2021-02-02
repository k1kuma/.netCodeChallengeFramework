using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBankSample.Domain;
using WorldWideBankSample.Dtos;

namespace WorldWideBankSample.Services
{
    public interface IFetchCustomerQuery
    {
        Task<CustomerDto> Fetch(int customerId);
    }

    public class FetchCustomerQuery : IFetchCustomerQuery
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public FetchCustomerQuery(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task<CustomerDto> Fetch(int customerId)
        {
            var customer = await _session.Query<Customer>().SingleAsync(x => x.CustomerId == customerId);

            return _mapper.Map<CustomerDto>(customer);
        }
    }
}
