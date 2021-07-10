using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
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
}