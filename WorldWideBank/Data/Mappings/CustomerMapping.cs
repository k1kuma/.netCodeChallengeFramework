using WorldWideBank.Domain;

namespace WorldWideBank.Data.Mappings
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
