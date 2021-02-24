using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using WorldWideBank.Domain;
using WorldWideBank.Dtos;
using WorldWideBank.CustomExceptions;


namespace WorldWideBank.Services
{
    public interface IPerformTransactionCommand
    {
        Task<AccountDto> Execute(string type, decimal amount, CustomerDto customerDto, string description,
            AccountDto accountDto, string currency = "CAD");
    }

    public class PerformTransactionCommand : IPerformTransactionCommand
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public PerformTransactionCommand(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task<AccountDto> Execute(string type, decimal amount, CustomerDto customerDto, string description,
            AccountDto accountDto, string currency = "CAD")
        {
            // var accounts = _session.Query<Customer>().SingleOrDefault(x => x.Id == customerDto.CustomerId)?.Accounts;
            // if (accounts == null)
            // {
            //     throw new AccountNotFoundException(accountDto.AccountNumber);
            // }
            // var account = accounts.Cast<Account>().SingleOrDefault(i =>
            //     i.AccountNumber == accountDto.AccountNumber);

            var account = await _session.Query<Account>().Where(x => x.AccountNumber == accountDto.AccountNumber).SingleOrDefaultAsync();
            if (account == null)
            {
                throw new AccountNotFoundException(accountDto.AccountNumber);
            }
            var currencyObj = new Currency { Code = "CAD", Name = "United States Dollar", Value = 100 };;
            if (currency == "USD")
            {
                currencyObj = new Currency { Code = "USD", Name = "United States Dollar", Value = 200 };
            }
            else if (currency == "MXN")
            {
                currencyObj = new Currency { Code = "MXN", Name = "Mexican Peso", Value =  10 };
            }

            var moneyObj = new Money { Value = (decimal) amount, Currency = currencyObj };

            type = type.ToLower();
            if (type.ToLower() == "withdraw")
            {
                account.Withdraw(moneyObj, _mapper.Map<Customer>(customerDto), description);
            }
            else if (type.ToLower() == "deposit")
            {
                account.Deposit(moneyObj, description);
            }

            // Save and return updated account
            await _session.SaveAsync(account);
            return _mapper.Map<AccountDto>(account);
        }
    }
}
