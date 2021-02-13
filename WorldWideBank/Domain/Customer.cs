using System.Collections.Generic;
using WorldWideBank.Domain.Core;

namespace WorldWideBank.Domain
{
    public class Customer: Entity
    {
        public virtual string Name { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual IList<Account> Accounts { get; protected set; }
    }
}
