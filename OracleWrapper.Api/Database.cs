namespace OracleWrapper.Api
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Persist;
    using BerkeleyDB;

    public class Database<TKey, TValue> : IDatabase<TKey, TValue>
    {
        public readonly string Path;
        public readonly IConverter<TKey> KeyConverter;
        public readonly IConverter<TValue> ValueConverter;
        public readonly BTreeDatabaseConfig Config;


        private BTreeDatabase database;

        public Database(string path, IConverter<TKey> keyConverter, IConverter<TValue> valueConverter, BTreeDatabaseConfig config)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path can't be null or white space");
            }

            this.Path = path;
            this.KeyConverter = keyConverter;
            this.ValueConverter = valueConverter;
            this.Config = config;

            this.database = BTreeDatabase.Open(this.Path, config);
        }

        public Database(string path)
            : this(path, new DefaultConverter<TKey>(), new DefaultConverter<TValue>(), new BTreeDatabaseConfig()
            {
                Creation = CreatePolicy.IF_NEEDED,
                Duplicates = DuplicatesPolicy.NONE,
                CacheSize = new CacheInfo(1, 1 * 1024, 2)
            })
        {
        }

        public TValue this[TKey key]
        {
            get
            {
                var cursor = this.database.Cursor();

                var keyEntry = ConvertKey(key);

                if (cursor.Move(keyEntry, true))
                {
                    return ValueConverter.FromBytes(cursor.Current.Value.Data);
                }

                throw new ArgumentException("Key doesn't present");
            }

            set
            {
                this.Add(key, value);
            }
        }

        public int Count
        {
            get
            {
                return (int)this.database.Stats().nKeys;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return this.Select(x => x.Key).ToList();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return this.Select(x => x.Value).ToList();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var keyEntry = ConvertKey(item.Key);
            var valueEntry = ConvertValue(item.Value);

            this.database.PutNoDuplicate(keyEntry, valueEntry);
        }

        public void Add(TKey key, TValue value)
        {
            var keyEntry = ConvertKey(key);
            var valueEntry = ConvertValue(value);

            this.database.Put(keyEntry, valueEntry);
        }

        public void Clear()
        {
            this.database.Truncate();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var key = ConvertKey(item.Key);
            var value = ConvertValue(item.Value);
            var kv = new KeyValuePair<DatabaseEntry, DatabaseEntry>(key, value);

            return this.database.Cursor().Move(kv, true);
        }

        public bool ContainsKey(TKey key)
        {
            return this.database.Cursor().Move(this.ConvertKey(key), true);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new ArgumentException("array length is lower than count of element in database");
            }

            var startIndex = arrayIndex;

            foreach (var kv in this)
            {
                array[startIndex++] = kv;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var cursor = this.database.Cursor();

            while (cursor.MoveNext())
            {
                var key = this.ConvertKey(cursor.Current.Key);
                var value = this.ConvertValue(cursor.Current.Value);

                yield return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            this.database.Delete(ConvertKey(key));

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var cursor = this.database.Cursor();

            if (cursor.Move(ConvertKey(key), true))
            {
                value = ConvertValue(cursor.Current.Value);

                return true;
            }

            value = default(TValue);

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Flush()
        {
            this.database.Sync();
        }

        public void Dispose()
        {
            this.database.Close(true);
        }

        private DatabaseEntry ConvertKey(TKey key)
        {
            var keyBuffer = this.KeyConverter.ToBytes(key);
            var keyEntry = new DatabaseEntry(keyBuffer);

            return keyEntry;
        }

        private TKey ConvertKey(DatabaseEntry key)
        {
            return this.KeyConverter.FromBytes(key.Data);
        }

        private DatabaseEntry ConvertValue(TValue value)
        {
            var valueBuffer = this.ValueConverter.ToBytes(value);
            var valueEntry = new DatabaseEntry(valueBuffer);

            return valueEntry;
        }

        private TValue ConvertValue(DatabaseEntry value)
        {
            return this.ValueConverter.FromBytes(value.Data);
        }
    }
}
