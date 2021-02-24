using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NHibernate;
using NUnit.Framework;
using WorldWideBank;
using WorldWideBank.Controllers;
using WorldWideBank.CustomExceptions;
using WorldWideBank.Dtos;
using WorldWideBank.Services;

namespace WorldWideBankTests.Integration
{
    class AccountsControllerSpec: IntegrationTest
    {
        [Test]
        public async Task WhenCreatingAnAccountwithOneOwner_ResultShouldBeOk()
        {
            var customer = new CustomerDto
            {
                CustomerId = 77,
                Name = "John Smith"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = (ICreateOrUpdateCustomerCommand) _server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);

            var accountToCreate = new AccountDto
            {
                AccountNumber = 654,
                Balance = 500.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> {customer}
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());
        }

        [Test]
        public async Task WhenCreatingAnAccountWithTwoOwners_ResultShouldBeOk()
        {
            var customer = new CustomerDto
            {
                CustomerId = 77,
                Name = "John Smith"
            };

            var customer2 = new CustomerDto
            {
                CustomerId = 93,
                Name = "Jane Doe"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);
            await _createOrUpdateCustomerCommand.Execute(customer2);

            var accountToCreate = new AccountDto
            {
                AccountNumber = 654,
                Balance = 500.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer, customer2 }
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());
        }

        [Test]
        public async Task WhenCreatingAnAccountThatAlreadyExists_ResultShouldNotBeOk()
        {
            var customer = new CustomerDto
            {
                CustomerId = 77,
                Name = "John Smith"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);

            var accountToCreate = new AccountDto
            {
                AccountNumber = 654,
                Balance = 500.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

            //First call passes
            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());

            //Second call fails
            Assert.That(() => _client.PostAsync("/Accounts", GetStringContent(accountToCreate)),
                Throws.TypeOf<AccountAlreadyExistsException>().With.Message.Contains(
                    "Account with Account Id of " +  accountToCreate.AccountNumber + " already exists."));
        }

        [Test]
        public async Task TestCaseOneFromAssessment_GoodCase()
        {
            // **Case 1:** 
            // Customer: Stewie Griffin Customer ID: 777 Account Number: 1234
            // Initial Balance for account number 1234: $100.00 CAD
            var customer = new CustomerDto
            {
                CustomerId = 777,
                Name = "Stewie Griffin"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = 
                (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);

            var accountToCreateAndUpdate = new AccountDto
            {
                AccountNumber = 1234,
                Balance = 100.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreateAndUpdate));
            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());

            // // Stewie Griffin deposits $300.00 USD to account number 1234.
            var amount = 300.0M;
            var currency = "USD";
            var description = "Stewie Griffin deposits $300.00 USD to account number 1234";

            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));
            var retrievedAccount = await _performTransactionCommand.Execute("deposit", amount, customer, description, accountToCreateAndUpdate, currency);

            Assert.AreEqual(1234, retrievedAccount.AccountNumber);
            Assert.AreEqual(700, retrievedAccount.Balance);
            Assert.AreEqual("CAD", retrievedAccount.CurrencyCode);

            IFetchAccountQuery _fetchAccountQuery = 
                (IFetchAccountQuery) _server.Services.GetService(typeof(IFetchAccountQuery));
        }

        // [Test]
        // public async Task TestCaseTwoFromAssessment_GoodCase()
        // {
        //     var customer = new CustomerDto
        //     {
        //         CustomerId = 777,
        //         Name = "Stewie Griffin"
        //     };

        //     ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = 
        //         (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

        //     await _createOrUpdateCustomerCommand.Execute(customer);

        //     var accountToCreate = new AccountDto
        //     {
        //         AccountNumber = 1234,
        //         Balance = 500.0M,
        //         CurrencyCode = "CAD",
        //         Owners = new List<CustomerDto> { customer }
        //     };

        //     // Need to perform Deposit now before retrieving account to pull-up details


        //     // Update BE with new account
        //     var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

        //     IFetchAccountQuery _fetchAccountQuery = 
        //         (IFetchAccountQuery) _server.Services.GetService(typeof(IFetchAccountQuery));

        //     var retrievedAccount = await _fetchAccountQuery.Fetch(accountToCreate.AccountNumber);

        // Console.WriteLine(accountToCreateAndUpdate.AccountNumber);
        // Console.WriteLine(accountToCreateAndUpdate.Balance);
        // Console.WriteLine(accountToCreateAndUpdate.CurrencyCode);

        // Console.WriteLine(retrievedAccount.AccountNumber);
        // Console.WriteLine(retrievedAccount.Balance);
        // Console.WriteLine(retrievedAccount.CurrencyCode);

        //     Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());
        //     Assert.AreEqual(accountToCreate.AccountNumber, retrievedAccount.AccountNumber);
        //     Assert.AreEqual(accountToCreate.Balance, retrievedAccount.Balance);
        //     Assert.AreEqual(accountToCreate.CurrencyCode, retrievedAccount.CurrencyCode);
        // }
    }
}
