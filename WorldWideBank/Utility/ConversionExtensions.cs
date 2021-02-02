using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Proxy;

namespace WorldWideBankSample.Utility
{
    public static class ConversionExtensions
    {
        public static TypeToDowncastTo DowncastTo<TypeToDowncastTo>(this object subject) where TypeToDowncastTo : class
        {
            if (subject is INHibernateProxy proxy)
            {
                return proxy.HibernateLazyInitializer.GetImplementation() as TypeToDowncastTo;
            }

            return subject as TypeToDowncastTo;
        }
    }
}
