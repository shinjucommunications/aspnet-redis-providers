namespace Microsoft.Web.Redis
{
    public interface ICustomSerializable
    {
        byte[] GetBytesFromObject(object data);
        object GetObjectFromBytes(byte[] dataAsBytes);
        bool MatchKey(string key, object data);
        bool MatchKey(string key);
    }
}