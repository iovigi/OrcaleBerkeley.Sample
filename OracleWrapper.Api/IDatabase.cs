namespace OracleWrapper.Api
{
    using System;
    using System.Collections.Generic;

    public interface IDatabase<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
    {
        void Flush();
    }
}
