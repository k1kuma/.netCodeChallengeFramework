using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldWideBankSample.Domain
{
    public class Money
    {
        public virtual decimal Value { get; init; }
        public virtual Currency Currency { get; init; }
    }
}
