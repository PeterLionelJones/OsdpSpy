using OsdpSpy.Abstractions;

namespace OsdpSpy.Models;

public class Payload : IPayload
{
    public byte[] Cipher { get; set; }
    public byte[] Plain { get; set; }
}