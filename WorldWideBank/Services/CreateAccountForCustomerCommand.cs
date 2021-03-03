using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBank.CustomExceptions;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;

namespace WorldWideBank.Services
{
    public interface ICreateAccountCommand
    {
        Task<AccountDto> Execute(AccountDto accountDto);
    }

    public class CreateAccountCommand : ICreateAccountCommand
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public CreateAccountCommand(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task<AccountDto> Execute(AccountDto accountDto)
        {
            var currency= await _session.Query<Currency>().SingleAsync(x => x.Code == accountDto.CurrencyCode);

            var account = await _session.Query<Account>().SingleOrDefaultAsync(x =>
                x.AccountNumber == accountDto.AccountNumber);

            if (account != null)
            {
                throw new AccountAlreadyExistsException(accountDto.AccountNumber);
            }

            var customerIds = accountDto.Owners.Select(x => x.CustomerId);
            var customers = await _session.Query<Customer>()
                .Where(x => customerIds.Contains(x.CustomerId)).ToListAsync();

            foreach (var accountDtoOwner in accountDto.Owners)
            {
                if (!customers.Any(x => x.CustomerId == accountDtoOwner.CustomerId))
                {
                    throw new CustomerNotFoundException(accountDtoOwner.CustomerId);
                }
            }

            account = new Account(accountDto.AccountNumber, currency, new Money { Currency = currency, Value = accountDto.Balance }, customers);

            await _session.SaveAsync(account);

            return _mapper.Map<AccountDto>(account);
        }
    }
}
