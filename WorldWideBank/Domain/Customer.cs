using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldWideBankSample.Domain.Core;

namespace WorldWideBankSample.Domain
{
    public class Customer: Entity
    {
        public virtual string Name { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual IList<Account> Accounts { get; protected set; }
    }
}
