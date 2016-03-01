namespace OracleWrapper.Api.Persist
{
    public interface IConverter<T>
    {
        byte[] ToBytes(T value);
        T FromBytes(byte[] value);
    }
}
