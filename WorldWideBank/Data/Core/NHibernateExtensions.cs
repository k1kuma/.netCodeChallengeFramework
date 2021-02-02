using Microsoft.Extensions.DependencyInjection;

namespace WorldWideBankSample.Data.Core
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var sessionFactory = new DatabaseSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.Create(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
