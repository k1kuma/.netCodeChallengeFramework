using FluentNHibernate.Mapping;
using WorldWideBankSample.Domain.Core;

namespace WorldWideBankSample.Data.Mappings
{
    public class NHibernateClassMapping<T> : ClassMap<T> where T : Entity
    {

        public NHibernateClassMapping()
        {
            base.Id(x => x.Id).UnsavedValue(Entity.DefaultID);
            Table($"{typeof(T).Name}s");
        }
    }
}
