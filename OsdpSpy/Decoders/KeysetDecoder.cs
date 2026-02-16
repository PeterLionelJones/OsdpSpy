using System;
using OsdpSpy.Annotations;
using OsdpSpy.Extensions;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public class KeysetDecoder : ICommandDecoder
{
    public Command Command => Command.KEYSET;

    public void Decode(byte[] input, IAnnotation output)
    {
        output
            .AppendItem("KeyType", input[0].ToKeyTypeString())
            .AppendItem("KeyBytes", input[1], "Bytes")
            .AppendItem("Scbk", input.ToScbk().ToHexString());
    }
}

internal static class KeysetDecoderExtensions
{
    internal static byte[] ToScbk(this byte[] input)
    {
        var scbk = new byte[16];
        Buffer.BlockCopy(input, 2, scbk, 0, 16);
        return scbk;
    }
        
    internal static string ToKeyTypeString(this byte value)
    {
        return value switch
        {
            0x01 => "Secure Channel Base Key",
            _ => "Unknown Key Type"
        };
    }
}