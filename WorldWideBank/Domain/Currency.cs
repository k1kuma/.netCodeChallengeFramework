using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldWideBankSample.Domain.Core;

namespace WorldWideBankSample.Domain
{
    public class Currency: Entity
    {
        public virtual string Code { get; init; }
        public virtual string Name { get; init; }
        public virtual int Value { get; set; }
    }
}
