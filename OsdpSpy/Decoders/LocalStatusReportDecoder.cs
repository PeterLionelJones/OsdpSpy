using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders
{
    public class LocalStatusReportDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.LSTATR;

        public void Decode(byte[] input, IAnnotation output)
        {
            output
                .AppendItem("TamperStatus", input[0].ToTamperStatusString())
                .AppendItem("PowerStatus", input[1].ToPowerStatusString());
        }
    }
    
    internal static class LocalStatusReportDecoderExtensions
    {
        internal static string ToTamperStatusString(this byte status)
        {
            return status switch 
            { 
                0 => "0 - Normal", 
                1 => "1 - Tamper", 
                _ => $"{status} - Unspecified"
            };
        }

        internal static string ToPowerStatusString(this byte status)
        {
            return status switch 
            { 
                0 => "0 - Normal", 
                1 => "1 - Power Failure", 
                _ => $"{status} - Unspecified"
            };
        }
    }
}