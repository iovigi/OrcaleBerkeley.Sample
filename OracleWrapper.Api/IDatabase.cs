namespace OracleWrapper.Api
{
    public interface IDatabase<TKey,TValue>
    {
        void Insert(TKey key, TValue value);
    }
}
