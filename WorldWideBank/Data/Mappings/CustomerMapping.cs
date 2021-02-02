using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldWideBankSample.Domain;

namespace WorldWideBankSample.Data.Mappings
{
    public class CustomerMapping: NHibernateClassMapping<Customer>
    {
        public CustomerMapping(): base()
        {
            HasManyToMany(x => x.Accounts);
            Map(x => x.Name);
            Map(x => x.CustomerId);
        }
    }
}
