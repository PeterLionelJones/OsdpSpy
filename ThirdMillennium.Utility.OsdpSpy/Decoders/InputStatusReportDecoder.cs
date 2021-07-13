using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class InputStatusReportDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.ISTATR;

        public void Decode(byte[] input, IAnnotation output)
        {
            for (var i = 0; i < input.Length && input[i] != 0x80; ++i)
            {
                output.AppendItem(
                    $"InputStatus{i + 1}", 
                    input[i].ToInputStatusString());
            }
        }
    }
    
    internal static class InputStatusReportDecoderExtensions
    {
        internal static string ToInputStatusString(this byte status)
        {
            return status switch 
            { 
                0 => "0 - Inactive", 
                1 => "1 - Active", 
                _ => $"{status} - Unspecified"
            };
        }
    }
}