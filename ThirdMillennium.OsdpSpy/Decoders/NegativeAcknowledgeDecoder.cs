using OsdpSpy.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class NegativeAcknowledgeDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.NAK;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendItem("ErrorCodeValue", input[0].ToNegativeAckowledgementString());
        }
    }

    internal static class NegativeAcknowledgeDecoderExtensions
    {
        internal static string ToNegativeAckowledgementString(this byte error)
        {
            return error switch
            {
                0x00 => "0x00 - No Error",
                0x01 => "0x01 - Message Check Error, CrC or Checksum",
                0x02 => "0x02 - Command Length Error",
                0x03 => "0x03 - Unknown Command Code",
                0x04 => "0x04 - Unexpected Sequence Number Detected in Header",
                0x05 => "0x05 - PD Does Not Support The Security Block",
                0x06 => "0x06 - Encrypted Communication is Required for this Command",
                0x07 => "0x07 - BIO_TYPE Not Supported",
                0x08 => "0x08 - BIO_FORMAT Not Supported",
                0x09 => "0x09 - Unable to Process Command",
                _ => $"0x{error:X02} - Unknown Error Code"
            };
        }
    }
}