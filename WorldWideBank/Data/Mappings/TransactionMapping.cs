using System.Security.Cryptography.X509Certificates;
using WorldWideBankSample.Domain;
using FluentNHibernate.Mapping;

namespace WorldWideBankSample.Data.Mappings
{
    public class TransactionMapping: NHibernateClassMapping<Transaction>
    {
        public TransactionMapping(): base()
        {
            Map(x => x.TransactionType);
            Component(x => x.Amount, c =>
            {
                c.Map(m => m.Value);
                c.References(m => m.Currency);
            });
            Map(x => x.DateTime);
            Map(x => x.Description);
        }
    }
}
