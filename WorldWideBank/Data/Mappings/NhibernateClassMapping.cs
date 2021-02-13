using FluentNHibernate.Mapping;
using WorldWideBank.Domain.Core;

namespace WorldWideBank.Data.Mappings
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
