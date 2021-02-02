using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WorldWideBankSample.Services;

namespace WorldWideBankSample.Utility.Ioc
{
    public static class IocExtensions
    {
        public static IServiceCollection AddCommandsAndQueries(this IServiceCollection services)
        {
            var servicesNamespace = typeof(FetchAccountQuery).Namespace;
            var allInterfaces = typeof(FetchAccountQuery).Assembly.GetTypes().Where(x => x.IsInterface && x.Name.StartsWith("I") && (x.Namespace?.Contains(servicesNamespace) ?? false));
            var allTypes = typeof(FetchAccountQuery).Assembly.GetTypes().Where(x => x.IsClass && (x.Namespace?.Contains(servicesNamespace) ?? false)).ToList();

            var interfaceAndClassList = from theInterface in allInterfaces
                let classToLookFor = theInterface.Name.Substring(1)
                let theClass = allTypes.SingleOrDefault(x => x.Name.Equals(classToLookFor))
                where theClass != null
                select new { theInterface, theClass };

            foreach (var interfaceAndClass in interfaceAndClassList)
            {
                services.AddScoped(interfaceAndClass.theInterface, interfaceAndClass.theClass);
            }

            return services;
        }
    }
}
