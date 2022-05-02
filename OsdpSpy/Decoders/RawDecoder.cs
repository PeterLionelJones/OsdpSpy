using System;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;
using ThirdMillennium.Protocol;

namespace OsdpSpy.Decoders
{
    public class RawDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.RAW;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendItem("ReaderNumber", input[0]);
            output.AppendItem("FormatCode", input[1].ToFormatString());
            output.AppendItem("BitCount", input.ToRawCardLength());
            output.AppendItem("Data", input.ToRawCardData().ToHexString());
        }
    }

    internal static class RawDecoderExtensions
    {
        internal static int ToRawCardLength(this byte[] input)
        {
            return input[2] | (input[3] << 8);
        }

        internal static byte[] ToRawCardData(this byte[] input)
        {
            var bits = input.ToRawCardLength();
            var bytes = 1 + (bits - 1) / 8;
            var data = new byte[bytes];
            Buffer.BlockCopy(input, 4, data, 0, bytes);
            return data;
        }

        internal static string ToRawCardString(this byte[] input)
        {
            return $"[ {input.ToRawCardLength()} ] {input.ToRawCardData().ToHexString()}";
        }
    
        internal static string ToFormatString(this byte format)
        {
            return format switch
            {
                0x00 => "Not Specified, Raw Bit Array",
                0x01 => "P/DATA/P, Wiegand",
                _ => "Unknown"
            };
        }
    }
}