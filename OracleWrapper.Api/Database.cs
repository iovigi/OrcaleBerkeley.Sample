namespace OracleWrapper.Api
{
    using System;

    public class Database<TKey, TValue>
    {
        public readonly string Path;

        public Database(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path can't be null or white space");
            }

            this.Path = path;
        }


    }
}
