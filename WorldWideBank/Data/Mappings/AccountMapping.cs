using WorldWideBank.Domain;

namespace WorldWideBank.Data.Mappings
{
    public class AccountMapping: NHibernateClassMapping<Account>
    {
        public AccountMapping(): base()
        {
            HasManyToMany(x => x.Owners);
            HasMany(x => x.Transactions).Cascade.All();
            Map(x => x.AccountNumber);
            Map(x => x.Balance);
            References(x => x.Currency);
        }
    }
}
