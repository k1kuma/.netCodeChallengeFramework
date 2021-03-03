using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorldWideBank.Dtos;
using WorldWideBank.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorldWideBank.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IFetchAccountQuery _fetchAccountQuery;
        private readonly IFetchAccountsQuery _fetchAccountsQuery;
        private readonly ICreateAccountCommand _createAccountCommand;
        private readonly IPerformTransactionCommand _performTransactionCommand;

        public AccountsController(IFetchAccountQuery fetchAccountQuery, IFetchAccountsQuery fetchAccountsQuery,
            ICreateAccountCommand createAccountCommand, IPerformTransactionCommand performTransactionCommand)
        {
            _fetchAccountQuery = fetchAccountQuery;
            _fetchAccountsQuery = fetchAccountsQuery;
            _createAccountCommand = createAccountCommand;
            _performTransactionCommand = performTransactionCommand;
        }

        /// <summary>
        /// Get all Accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ICollection<AccountDto>> Get()
        {
            return await _fetchAccountsQuery.Fetch();
        }

        /// <summary>
        /// Get account with account Number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet("{accountNumber}")]
        public async Task<AccountDto> Get(int accountNumber)
        {
            return await _fetchAccountQuery.Fetch(accountNumber);
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="accountDto">Account Details</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AccountDto> Post(AccountDto accountDto)
        {
            return await _createAccountCommand.Execute(accountDto);
        }

        // /// <summary>
        // /// Perform Transaction on Account
        // /// </summary>
        // /// <returns></returns>
        // [HttpPost]
        // public async Task<AccountDto> PerformTransaction(string type, decimal amount, CustomerDto customerDto, string description,
        //     AccountDto accountDto, string currency = "CAD", AccountDto toAccountDto = null)
        // {
        //     return await _performTransactionCommand.PerformTransaction(type, amount, customerDto, description, accountDto, currency, toAccountDto);
        // }

    }
}
