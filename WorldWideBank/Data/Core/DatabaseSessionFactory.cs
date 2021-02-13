using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using WorldWideBank.Data.Mappings;

namespace WorldWideBank.Data.Core
{
    public interface IDatabaseSessionFactory
    {
        ISession Create(string connectionString);
    }

    public class DatabaseSessionFactory : IDatabaseSessionFactory
    {
        private static ISessionFactory sessionFactory;
        private static object mutex = new object();

        public ISession Create(string connectionString)
        {
            if (null == sessionFactory)
            {
                lock (mutex)
                {
                    if (null == sessionFactory)
                    {
                        sessionFactory = create_session_factory(connectionString);
                    }
                }
            }
            var session = sessionFactory.OpenSession();
            session.FlushMode = FlushMode.Manual;

            return session;
        }

        ISessionFactory create_session_factory(string connectionString)
        {
            return Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard
                        .ConnectionString(connectionString)
                )

                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<AccountMapping>())
                .Mappings(x => x.FluentMappings.ExportTo(AppDomain.CurrentDomain.BaseDirectory))
                    .ExposeConfiguration(x => new SchemaExport(x).Execute(true, true, false))
                .BuildSessionFactory();
        }
    }
}
