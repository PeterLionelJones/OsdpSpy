using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class OutputStatusReportDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.OSTATR;

        public void Decode(byte[] input, IAnnotation output)
        {
            for (var i = 0; i < input.Length && input[i] != 0x80; ++i)
            {
                output.AppendItem(
                    $"OutputStatus{i + 1}", 
                    input[i].ToInputStatusString()); 
            }
        }
    }
    
    internal static class OutputStatusReportDecoderExtensions
    {
        internal static string ToOutputStatusString(this byte status)
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