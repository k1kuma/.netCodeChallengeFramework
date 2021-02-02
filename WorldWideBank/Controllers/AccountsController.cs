using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WorldWideBankSample.Domain;
using WorldWideBankSample.Dtos;
using WorldWideBankSample.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorldWideBankSample.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IFetchAccountQuery _fetchAccountQuery;
        private readonly IFetchAccountsQuery _fetchAccountsQuery;
        private readonly ICreateAccountCommand _createAccountCommand;

        public AccountsController(IFetchAccountQuery fetchAccountQuery, IFetchAccountsQuery fetchAccountsQuery, ICreateAccountCommand createAccountCommand)
        {
            _fetchAccountQuery = fetchAccountQuery;
            _fetchAccountsQuery = fetchAccountsQuery;
            _createAccountCommand = createAccountCommand;
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
    }
}
