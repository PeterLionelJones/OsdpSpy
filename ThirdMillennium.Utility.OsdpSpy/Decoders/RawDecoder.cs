using System;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class RawDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.RAW;

        public void Decode(byte[] input, IAnnotation output)
        {
            var bits = input[2] | (input[3] << 8);
            var bytes = 1 + (bits - 1) / 8;
            var data = new byte[bytes];
            Buffer.BlockCopy(input, 4, data, 0, bytes);

            output.AppendItem("ReaderNumber", input[0]);
            output.AppendItem("FormatCode", input[1].ToFormatString());
            output.AppendItem("BitCount", bits);
            output.AppendItem("Data", data.ToHexString());
        }
        
    }
}