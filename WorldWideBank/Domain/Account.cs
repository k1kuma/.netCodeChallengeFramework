﻿using System;
using System.Collections.Generic;
using System.Linq;
using WorldWideBank.Domain.Core;

namespace WorldWideBank.Domain
{
    public class Account: Entity
    {
        public virtual IList <Customer> Owners { get; init; }
        public virtual decimal Balance { get; protected set; } = 0.0M;
        public virtual IList<Transaction> Transactions { get; init; } = new List<Transaction>();
        public virtual Currency Currency { get; init; }
        public virtual int AccountNumber { get; init; }

        protected Account()
        {

        }

        public Account(int accountNumber, Currency currency, Money initalBalance, IEnumerable<Customer> owners)
        {
            AccountNumber = accountNumber;
            Owners = owners.ToList();
            Currency = currency;
            Deposit(initalBalance, "Initial Deposit");
        }

        public virtual Transaction Deposit(Money amount, string description)
        {
            var transaction = CreateTransaction(amount, TransactionType.Debit, description);

            Transactions.Add(transaction);
            Balance += transaction.Amount.Value;

            return transaction;
        }

        public virtual Transaction Withdraw(Money amount, Customer owner, string description)
        {
            var transactionToAttempt = CreateTransaction(amount, TransactionType.Credit, description);

            if (!Owners.Contains(owner))
            {
                throw new InvalidOperationException($"Customer {owner.CustomerId} not authorized to perform withdraw or transfer out of this account!");
            }

            if (transactionToAttempt.Amount.Value <= Balance && Owners.Contains(owner))
            {
                Transactions.Add(transactionToAttempt);
                Balance -= transactionToAttempt.Amount.Value;
                return transactionToAttempt;
            }

            throw new InvalidOperationException("Not enough money in your account or you do not have access to this account");
        }

        private Transaction CreateTransaction(Money amount, TransactionType transactionType, string description)
        {
            var transactionDescription = $"{description} ({amount.Value} {amount.Currency.Code}).";
            var amountInAccountCurrency = new Money{Value = (amount.Value * amount.Currency.Value) / Currency.Value, Currency = Currency};
            return new Transaction
                { DateTime = DateTime.UtcNow, Amount = amountInAccountCurrency, TransactionType = transactionType, Description = transactionDescription };

        }
    }
}
