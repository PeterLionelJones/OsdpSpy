using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class KeypadDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.KEYPAD;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendItem("ReaderNumber", input[0]);
            output.AppendItem("DigitCount", input[1]);
            for (var i = 0; i < input[1]; ++i)
            {
                output.AppendItem($"Key{1 + i}", input[2 + i].ToKeyString());
            }
        }
    }

    internal static class KeypadDecoderExtensions
    {
        internal static string ToKeyString(this byte key)
        {
            return key switch
            {
                0x0D => "#",
                0x41 => "F1",
                0x42 => "F2",
                0x43 => "F3",
                0x44 => "F4",
                0x45 => "F1 + F2",
                0x46 => "F2 + F3",
                0x47 => "F3 + F4",
                0x48 => "F1 + F4",
                0x7F => "*",
                _ => $"{(char) key}"
            };
        }
    }
}