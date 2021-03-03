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
        Task<AccountDto> PerformTransaction(string type, decimal amount, CustomerDto customerDto, string description,
            AccountDto accountDto, string currency = "CAD", AccountDto toAccountDto = null);
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

        private async Task<Account> retrieveAccount(int accountNumber) {
            var account = await _session.Query<Account>().Where(x =>
                x.AccountNumber == accountNumber).SingleOrDefaultAsync();
            if (account == null)
            {
                throw new AccountNotFoundException(accountNumber);
            }

            return account;
        }

        private async Task<Customer> retrieveCustomer(int customerID)
        {
            var customer = await _session.Query<Customer>()
                .SingleOrDefaultAsync(x => x.CustomerId == customerID) 
                    ?? new Customer { CustomerId = customerID };
            if (customer == null)
            {
                throw new CustomerNotFoundException(customerID);
            }
            return customer;
        }
 
        public async Task<AccountDto> PerformTransaction(string type, decimal amount, CustomerDto customerDto, string description,
            AccountDto accountDto, string currency = "CAD", AccountDto toAccountDto = null)
        {
            var account = await retrieveAccount(accountDto.AccountNumber);
            var customer = await retrieveCustomer(customerDto.CustomerId);

            Currency currencyObj = new Currency { Code = "CAD", Name = "United States Dollar", Value = 100 };;
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
            if (type == "withdraw")
            {
                account.Withdraw(moneyObj, customer, description);
            }
            else if (type == "deposit")
            {
                account.Deposit(moneyObj, description);
            }
            else if (type == "transfer") {
                // For Transfer, retrieve toAccount and credential before performing withdraw on 
                // fromAccount + deposit to toAccount.
                var toAccount = await retrieveAccount(toAccountDto.AccountNumber);
                account.Withdraw(moneyObj, customer, description);
                toAccount.Deposit(moneyObj, description);
                await _session.SaveAsync(toAccount);
            }
            else
            {
                throw new InvalidTransactionException(type);
            }
            // Save and return updated account
            await _session.SaveAsync(account);
            return _mapper.Map<AccountDto>(account);
        }
    }
}
