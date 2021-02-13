using System;
using WorldWideBank.Domain.Core;

namespace WorldWideBank.Domain
{
    public enum TransactionType { Debit, Credit };

    public class Transaction: Entity
    {
        public virtual DateTime DateTime { get; init; }
        public virtual Money Amount { get; init; }
        public virtual TransactionType TransactionType { get; init; }
        public virtual string Description { get; init; }
    }

}
