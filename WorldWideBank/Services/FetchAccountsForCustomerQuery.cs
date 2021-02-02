using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NHibernate;
using WorldWideBankSample.Domain;
using WorldWideBankSample.Dtos;

namespace WorldWideBankSample.Services
{
    public interface IFetchAccountsForCustomerQuery
    {
        IEnumerable<AccountDto> Fetch(long customerId);
    }

    public class FetchAccountsForCustomerQuery : IFetchAccountsForCustomerQuery
    {
        private readonly IMapper _mapper;
        private readonly ISession _session;

        public FetchAccountsForCustomerQuery(IMapper mapper, ISession session)
        {
            _mapper = mapper;
            _session = session;
        }
        public IEnumerable<AccountDto> Fetch(long customerId)
        {
            var accounts = _session.Query<Customer>().SingleOrDefault(x => x.Id == customerId)?.Accounts;
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        } 
    }
}
