using WorldWideBank.Domain.Core;

namespace WorldWideBank.Domain
{
    public class Currency: Entity
    {
        public virtual string Code { get; init; }
        public virtual string Name { get; init; }
        public virtual int Value { get; set; }
    }
}
