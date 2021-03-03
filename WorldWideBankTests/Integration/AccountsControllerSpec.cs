using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using WorldWideBank.CustomExceptions;
using WorldWideBank.Dtos;
using WorldWideBank.Services;
using WorldWideBank.Controllers;

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
        public async Task AttemptUnSupportedTransaction_ResultShouldNotBeOk()
        {
            var customer = new CustomerDto
            {
                CustomerId = 225,
                Name = "Matt Kikuchi"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);

            var accountToCreate = new AccountDto
            {
                AccountNumber = 789,
                Balance = 500.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));

            var transactionName = "checkcashing";
            var amount = 5000.0M;
            var currency = "MXN";
            var description = "Matt Kikuchi attempts unsupport transaction.";

            // Ensure InvalidTransactionException is thrown
            Assert.That(() => _performTransactionCommand.PerformTransaction(transactionName, amount, customer, description, accountToCreate, currency),
                Throws.TypeOf<InvalidTransactionException>().With.Message.Contains(
                    "Transaction " +  transactionName + " was not found"));
        }

        [Test]
        public async Task AttemptOverDraftWithdraw_ResultShouldNotBeOk()
        {
            var customer = new CustomerDto
            {
                CustomerId = 226,
                Name = "Matt Kikuchi"
            };

            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            await _createOrUpdateCustomerCommand.Execute(customer);

            var accountToCreate = new AccountDto
            {
                AccountNumber = 2021,
                Balance = 500.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var response = await _client.PostAsync("/Accounts", GetStringContent(accountToCreate));

            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));

            var transactionName = "withdraw";
            var amount = 5000.0M;
            var currency = "USD";
            var description = "Matt Kikuchi attempts overdraft withdraw";

            // Ensure InvalidOperationException is thrown
            Assert.That(() => _performTransactionCommand.PerformTransaction(transactionName, amount, customer, description, accountToCreate, currency),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains(
                    "Not enough money in your account or you do not have access to this account"));

            // **Output 5:** Account Number: 1010 Balance: $7,425.00 CAD
            IFetchAccountQuery _fetchAccountQuery = 
                (IFetchAccountQuery) _server.Services.GetService(typeof(IFetchAccountQuery));
            var retrievedAccount = await _fetchAccountQuery.Fetch(accountToCreate.AccountNumber);
            Assert.AreEqual(2021, retrievedAccount.AccountNumber);
            Assert.AreEqual(500.00, retrievedAccount.Balance);
            Assert.AreEqual("CAD", retrievedAccount.CurrencyCode);
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
            var retrievedAccount = await _performTransactionCommand.PerformTransaction("deposit", amount, customer, description, accountToCreateAndUpdate, currency);

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
            await _performTransactionCommand.PerformTransaction("withdraw", amount, customer, description, accountToCreate, currency);

            // Glenn Quagmire withdraws $12,500.00 USD from account number 2001
            amount = 12500.0M;
            currency = "USD";
            description = "Glenn Quagmire withdraws $12,500.00 USD from account number 2001";
            await _performTransactionCommand.PerformTransaction("withdraw", amount, customer, description, accountToCreate, currency);

            // Glenn Quagmire deposits $300.00 CAD to account number 2001.
            amount = 300.0M;
            currency = "CAD";
            description = "Glenn Quagmire deposits $300.00 CAD to account number 2001";
            var retrievedAccount = await _performTransactionCommand.PerformTransaction("deposit", amount, customer, description, accountToCreate, currency);

            // **Output 2:** Account Number: 2001 Balance: $9,800 CAD
            Assert.DoesNotThrow(() => response.EnsureSuccessStatusCode());
            Assert.AreEqual(2001, retrievedAccount.AccountNumber);
            Assert.AreEqual(9800.0M, retrievedAccount.Balance);
            Assert.AreEqual("CAD", retrievedAccount.CurrencyCode);
        }

        [Test]
        public async Task TestCaseThreeFromAssessment_GoodCase()
        {
            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = 
                (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));
            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));
            IFetchAccountQuery _fetchAccountQuery = 
                (IFetchAccountQuery) _server.Services.GetService(typeof(IFetchAccountQuery));

            // **Case 3:** Customer: Joe Swanson Customer ID: 002
            var customer = new CustomerDto
            {
                CustomerId = 002,
                Name = "Joe Swanson"
            };

            await _createOrUpdateCustomerCommand.Execute(customer);

            // Account Number: 1010 Initial balance for account number 1010: $7,425.00 CAD
            var account1010 = new AccountDto
            {
                AccountNumber = 1010,
                Balance = 7425.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var firstResponse = await _client.PostAsync("/Accounts", GetStringContent(account1010));
            Assert.DoesNotThrow(() => firstResponse.EnsureSuccessStatusCode());

            // Customer: Joe Swanson Customer ID: 002 Account Number: 5500 Initial balance for account number
            // 5500: $15,000.00 CAD
            var account5500 = new AccountDto
            {
                AccountNumber = 5500,
                Balance = 15000.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { customer }
            };

            var secondResponse = await _client.PostAsync("/Accounts", GetStringContent(account5500));
            Assert.DoesNotThrow(() => secondResponse.EnsureSuccessStatusCode());

            // Joe Swanson withdraws $5,000.00 CAD from account number 5500.
            var amount = 5000.0M;
            var currency = "CAD";
            var description = "Joe Swanson withdraws $5,000.00 CAD from account number 5500";
            await _performTransactionCommand.PerformTransaction("withdraw", amount, customer, description, account5500, currency);

            // Joe Swanson transfers $7,300.00
            // CAD from account number 1010 to account number 5500.
            amount = 7300.0M;
            currency = "CAD";
            description = "Joe Swanson transfers $7,300.00 CAD from account number 1010 to account number 5500.";
            await _performTransactionCommand.PerformTransaction("transfer", amount, customer, description, account1010, currency, account5500);

            // Joe Swanson deposits $13,726.00 MXN to account number 1010.
            amount = 13726.0M;
            currency = "MXN";
            description = "Joe Swanson deposits $13,726.00 MXN to account number 1010";

            var retrievedAccount1 = await _performTransactionCommand.PerformTransaction("deposit", amount, customer, description, account1010, currency);
            var retrieveAccount2 = await _fetchAccountQuery.Fetch(account5500.AccountNumber);

            // **Output 3:** Account Number: 1010 Balance: $ 1,497.60 CAD
            //               Account Number: 5500 Balance: $17,300.00 CAD
            Assert.AreEqual(1010, retrievedAccount1.AccountNumber);
            Assert.AreEqual(1497.60, retrievedAccount1.Balance);
            Assert.AreEqual("CAD", retrievedAccount1.CurrencyCode);
            Assert.AreEqual(5500, retrieveAccount2.AccountNumber);
            Assert.AreEqual(17300.00, retrieveAccount2.Balance);
            Assert.AreEqual("CAD", retrieveAccount2.CurrencyCode);
        }

        [Test]
        public async Task TestCaseFourFromAssessment_GoodCase()
        {
            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = 
                (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));

            IFetchAccountQuery _fetchAccountQuery = 
                (IFetchAccountQuery) _server.Services.GetService(typeof(IFetchAccountQuery));

            // **Case 4:**
            // Customer: Peter Griffin Customer ID: 123 Account Number: 0123
            // Customer: Lois Griffin Customer ID: 456 Account Number: 0456
            var peterGriffin = new CustomerDto
            {
                CustomerId = 123,
                Name = "Peter Griffin"
            };
            var loisGriffin = new CustomerDto
            {
                CustomerId = 123,
                Name = "Lois Griffin"
            };

            await _createOrUpdateCustomerCommand.Execute(peterGriffin);
            await _createOrUpdateCustomerCommand.Execute(loisGriffin);

            // Initial balance for account number 0123: $150.00 CAD
            // Initial balance for account number 0456: $65,000.00 CAD
            var accountPG = new AccountDto
            {
                AccountNumber = 0123,
                Balance = 150.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { peterGriffin }
            };

            var accountLG = new AccountDto
            {
                AccountNumber = 0456,
                Balance = 65000.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { loisGriffin }
            };

            var firstResponse = await _client.PostAsync("/Accounts", GetStringContent(accountPG));
            var secondResponse = await _client.PostAsync("/Accounts", GetStringContent(accountLG));
            Assert.DoesNotThrow(() => firstResponse.EnsureSuccessStatusCode());
            Assert.DoesNotThrow(() => secondResponse.EnsureSuccessStatusCode());

            // Peter Griffin withdraws $70.00 USD from account number 0123.
            var amount = 70.0M;
            var currency = "USD";
            var description = "Peter Griffin withdraws $70.00 USD from account number 0123";
            await _performTransactionCommand.PerformTransaction("withdraw", amount, peterGriffin, description, accountPG, currency);

            // Lois Griffin deposits $23,789.00 USD to account number 0456.
            amount = 23789.0M;
            currency = "USD";
            description = "Lois Griffin deposits $23,789.00 USD to account number 0456";
            await _performTransactionCommand.PerformTransaction("deposit", amount, loisGriffin, description, accountLG, currency);

            // Lois Griffin transfers $23.75 CAD from account number 0456 to Peter Griffin (account number 0123).
            amount = 23.75M;
            currency = "CAD";
            description = "Lois Griffin transfers $23.75 CAD from account number 0456 to account number 0123";
            var account0456 = await _performTransactionCommand.PerformTransaction("transfer", amount, loisGriffin, description, accountLG, currency, accountPG);
            var account0123 = await _fetchAccountQuery.Fetch(accountPG.AccountNumber);

            // **Output 4:** Account Number: 0123 Balance: $33.75 CAD
            //               Account Number: 0456 Balance: $112,554.25 CAD
            Assert.AreEqual(0123, account0123.AccountNumber);
            Assert.AreEqual(33.75, account0123.Balance);
            Assert.AreEqual("CAD", account0123.CurrencyCode);
            Assert.AreEqual(0456, account0456.AccountNumber);
            Assert.AreEqual(112554.25, account0456.Balance);
            Assert.AreEqual("CAD", account0456.CurrencyCode);
        }

        [Test]
        public async Task TestCaseFiveFromAssessment_GoodCase()
        {
            ICreateOrUpdateCustomerCommand _createOrUpdateCustomerCommand = 
                (ICreateOrUpdateCustomerCommand)_server.Services.GetService(typeof(ICreateOrUpdateCustomerCommand));

            IPerformTransactionCommand _performTransactionCommand = 
                (IPerformTransactionCommand) _server.Services.GetService(typeof(IPerformTransactionCommand));

            IFetchAccountQuery _fetchAccountQuery = 
                (IFetchAccountQuery) _server.Services.GetService(typeof(IFetchAccountQuery));

            //  **Case 5:**
            // Customer: Joe Swanson Customer ID: 002
            var joeSwanson = new CustomerDto
            {
                CustomerId = 123,
                Name = "Joe Swanson"
            };
            var johnStark = new CustomerDto
            {
                CustomerId = 219,
                Name = "John Stark"
            };

            await _createOrUpdateCustomerCommand.Execute(joeSwanson);
            await _createOrUpdateCustomerCommand.Execute(johnStark);

            // Account Number: 1010 Initial balance for account number 1010: $7,425.00 CAD
            var account1010 = new AccountDto
            {
                AccountNumber = 1010,
                Balance = 7425.0M,
                CurrencyCode = "CAD",
                Owners = new List<CustomerDto> { joeSwanson }
            };

            var firstResponse = await _client.PostAsync("/Accounts", GetStringContent(account1010));
            Assert.DoesNotThrow(() => firstResponse.EnsureSuccessStatusCode());

            // Famous social engineer and thief John Shark (Customer ID 219) attempts to withdraw $100 USD from
            // account 1010. The bank determines that the account is not John’s and refuses to give him the money.
            // Optional: The bank notifies Joe Swanson that an unauthorized user attempted to withdraw money.
            var amount = 100.0M;
            var currency = "USD";
            var description = "Famous social engineer and thief John Shark (Customer ID 219) attempts to withdraw $100 USD from account 1010";

            // Ensure InvalidOperationException is thrown
            Assert.That(() => _performTransactionCommand.PerformTransaction("withdraw", amount, johnStark, description, account1010, currency),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains(
                    "Customer " +  johnStark.CustomerId + " not authorized to perform withdraw or transfer out of this account!"));

            // **Output 5:** Account Number: 1010 Balance: $7,425.00 CAD
            var retrievedAccount = await _fetchAccountQuery.Fetch(account1010.AccountNumber);
            Assert.AreEqual(1010, retrievedAccount.AccountNumber);
            Assert.AreEqual(7425.00, retrievedAccount.Balance);
            Assert.AreEqual("CAD", retrievedAccount.CurrencyCode);
        }
    }
}
