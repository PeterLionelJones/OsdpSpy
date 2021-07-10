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
            output.AppendItem("ReaderNumber", input[0]);
            output.AppendItem("FormatCode", input[1].ToFormatString());
            output.AppendItem("BitCount", input.ToRawCardLength());
            output.AppendItem("Data", input.ToRawCardData().ToHexString());
        }
    }
}