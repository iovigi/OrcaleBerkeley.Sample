namespace OracleWrapper.Api.Persist
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class DefaultConverter<T> : IConverter<T>
    {
        public T FromBytes(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream(value))
            {
                return (T)binaryFormatter.Deserialize(ms);
            }
        }

        public byte[] ToBytes(T value)
        {
            if (value == null)
            {
                return new byte[0];
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, value);

                return ms.ToArray();
            }
        }
    }
}
