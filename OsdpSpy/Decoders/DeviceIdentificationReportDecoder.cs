using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders
{
    public class DeviceIdentificationReportDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.PDID;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendItem("VendorId", input.ToVendorIdString());
            output.AppendItem("ModelNumber", input[3]);
            output.AppendItem("Version", input[4]);
            output.AppendItem("SerialNumber", input.ToSerialNumberString());
            output.AppendItem("FirmwareVersion", input.ToFirmwareVersionString());
        }
    }

    internal static class DeviceIdentificationReportDecoderExtensions
    {
        internal static string ToVendorIdString(this byte[] input)
        {
            return $"{input[0]:X02}:{input[1]:X02}:{input[2]:X02}";
        }

        internal static string ToSerialNumberString(this byte[] input)
        {
            return $"{input[5]:X02}{input[6]:X02}{input[7]:X02}{input[8]:X02}";
        }
        
        internal static string ToFirmwareVersionString(this byte[] input)
        {
            return $"{input[9]}.{input[10]:D02}.{input[11]:D02}";
        }
    }
}