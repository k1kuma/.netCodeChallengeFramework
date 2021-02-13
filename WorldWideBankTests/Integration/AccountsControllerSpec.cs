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

            //First Call Passes
            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());

            //Second call fails
            Assert.That(() => _client.PostAsync("/Accounts", GetStringContent(accountToCreate)), Throws.TypeOf<AccountAlreadyExistsException>().With.Message.Contains("Account with Account Id of 654 already exists."));
        }
    }
}
