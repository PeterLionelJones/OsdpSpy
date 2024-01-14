namespace OsdpSpy.Annotators;

public interface ISecureChannelSink
{
    void OnAuthenticating(int address, bool isDefault, byte[] scbk);
    void OnAuthenticationSychronised(int address);
    void OnAuthenticationLost(int address);
}