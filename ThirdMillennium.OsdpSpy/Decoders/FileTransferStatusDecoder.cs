using System;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class FileTransferStatusDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.FTSTAT;

        public void Decode(byte[] input, IAnnotation output)
        {
            var delay = input[1] | (input[2] << 8);
            var detail = (short)(input[3] | (input[4] << 8));
            var maxMessage = input[5] | (input[6] << 8);

            output
                .AppendItem("CanInterleave", (input[0] & 1) == 1)
                .AppendItem("LeaveSecureChannel", (input[0] & 2) == 2)
                .AppendItem("PollResponseAvailable", (input[0] & 4) == 4)
                .AppendItem("NextMessageDelay", delay, "Milliseconds")
                .AppendItem("Detail", detail.ToDetailString())
                .AppendItem("UpdateMaximumMessage", maxMessage, "Bytes");
        }
    }

    internal static class FileTransferStatusDecoderExtensions
    {
        internal static string ToDetailString(this short detail)
        {
            return detail switch
            {
                -3 => "-3 - File Data Unacceptable - Malformed",
                -2 => "-2 - Unrecognised File Contents",
                -1 => "-1 - Abort File Transfer",
                0 => "0 - OK to Proceed",
                1 => "1 - File Contents Processed",
                2 => "2 - Rebooting Now, Expect Full Communication Reset",
                3 => "3 - Finishing File Transfer, Send Idle Messages",
                _ => $"{detail} - Reserved for Future Use"
            };
        }
    }
}