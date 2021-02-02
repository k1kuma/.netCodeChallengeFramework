using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using WorldWideBankSample.Domain.Core;

namespace WorldWideBankSample.Domain
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
