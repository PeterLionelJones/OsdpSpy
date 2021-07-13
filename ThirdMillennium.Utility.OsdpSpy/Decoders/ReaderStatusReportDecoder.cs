using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class ReaderStatusReportDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.RSTATR;

        public void Decode(byte[] input, IAnnotation output)
        {
            for (var i = 0; i < input.Length && input[i] != 0x80; ++i)
            {
                output.AppendItem(
                    $"ReaderStatus{i + 1}", input[1].ToReaderStatusString());
            }
        }
    }

    internal static class ReaderStatusReportDecoderExtensions
    {
        internal static string ToReaderStatusString(this byte status)
        {
            return status switch 
            { 
                0 => "0 - Normal", 
                1 => "1 - Not Connected", 
                2 => "2 - Tamper",
                _ => $"{status} - Unspecified"
            };
        }
    }
}