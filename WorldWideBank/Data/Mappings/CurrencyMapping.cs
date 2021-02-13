using WorldWideBank.Domain;

namespace WorldWideBank.Data.Mappings
{
    public class CurrencyMapping: NHibernateClassMapping<Currency>
    {
        public CurrencyMapping(): base()
        {
            Map(x => x.Value);
            Map(x => x.Code);
            Map(x => x.Name);
        }
    }
}
