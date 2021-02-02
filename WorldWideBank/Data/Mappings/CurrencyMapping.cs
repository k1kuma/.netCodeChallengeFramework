using System.Security.Cryptography.X509Certificates;
using WorldWideBankSample.Domain;
using FluentNHibernate.Mapping;

namespace WorldWideBankSample.Data.Mappings
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
