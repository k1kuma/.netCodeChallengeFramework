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
            Assert.AreEqual(700.0M, retrievedAccount.Balance);
            Assert.AreEqual("CAD", retrievedAccount.CurrencyCode);
        }

        [Test]
        public async Task TestCaseTwoFromAssessment_GoodCase()
        {
            // **Case 2:** Customer: Glenn Quagmire Customer ID: 504 Account Number: 2001
            var customer = new CustomerDto
            {
                CustomerId = 504,
                Name = "Glenn Quagmire"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = 
                (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);

            // Initial balance for account number 2001: $35,000.00 CAD
            var accountToCreate = new AccountDto
            {
                AccountNumber = 2001,
                Balance = 35000.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));

            // Glenn Quagmire withdraws $5,000.00 MXN from account number 2001. 
            var amount = 5000.0M;
            var currency = "MXN";
            var description = "Glenn Quagmire withdraws $5,000.00 MXN from account number 2001";
            var retrievedAccount = await _performTransactionCommand.Execute("withdraw", amount, customer, description, accountToCreate, currency);

            // Glenn Quagmire withdraws $12,500.00 USD from account number 2001
            amount = 12500.0M;
            currency = "USD";
            description = "Glenn Quagmire withdraws $12,500.00 USD from account number 2001";
            retrievedAccount = await _performTransactionCommand.Execute("withdraw", amount, customer, description, accountToCreate, currency);

            // Glenn Quagmire deposits $300.00 CAD to account number 2001.
            amount = 300.0M;
            currency = "CAD";
            description = "Glenn Quagmire deposits $300.00 CAD to account number 2001";
            retrievedAccount = await _performTransactionCommand.Execute("deposit", amount, customer, description, accountToCreate, currency);

            // **Output 2:** Account Number: 2001 Balance: $9,800 CAD
            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());
            Assert.AreEqual(2001, retrievedAccount.AccountNumber);
            Assert.AreEqual(9800.0M, retrievedAccount.Balance);
            Assert.AreEqual("CAD", retrievedAccount.CurrencyCode);
        }
    }
}
