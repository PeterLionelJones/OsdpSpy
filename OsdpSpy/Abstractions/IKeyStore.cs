namespace OsdpSpy.Abstractions;

public interface IKeyStore
{
    void Clear();
    byte[] DefaultBaseKey { get; }
    byte[] Find(byte[] cUid);
    void List();
    void Store(byte[] cUid, byte[] key);
}