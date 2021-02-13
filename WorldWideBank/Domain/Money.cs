namespace WorldWideBank.Domain
{
    public class Money
    {
        public virtual decimal Value { get; init; }
        public virtual Currency Currency { get; init; }
    }
}
