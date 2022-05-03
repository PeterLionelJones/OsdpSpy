namespace OsdpSpy
{
    public interface IKeyStore
    {
        void Store(byte[] cUid, byte[] key);
        byte[] Find(byte[] cUid);
        void List();
        byte[] DefaultBaseKey { get; }
    }
}