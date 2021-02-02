using System.Security.Cryptography.X509Certificates;
using WorldWideBankSample.Domain;
using FluentNHibernate.Mapping;

namespace WorldWideBankSample.Data.Mappings
{
    public class AccountMapping: NHibernateClassMapping<Account>
    {
        public AccountMapping(): base()
        {
            HasManyToMany(x => x.Owners);
            HasMany(x => x.Transactions);
            Map(x => x.AccountNumber);
            Map(x => x.Balance);
            References(x => x.Currency);
        }
    }
}
