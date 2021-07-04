using System;
using System.Text;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public static class FrameProductExtensions
    {
        public static int Sequence(this IFrameProduct product)
            => product.Frame.Sequence;
        
        public static string CheckMethod(this IFrameProduct product)
            => product.Frame.UseCRC16 ? "CRC16" : "Checksum";

        public static bool HasSecurityControlBlock(this IFrameProduct product)
            => product.Frame.SecurityBlock != null;

        public static string SecurityControlBlock(this IFrameProduct product)
            => product.Frame.SecurityBlock.ToHexString();

        private static bool IsCipher(this IFrameProduct product)
        {
            return product.Frame.IsCp 
                ? product.HasSecurityControlBlock() && 
                  !product.Frame.Command.IsSecureChannelCommand() 
                : product.HasSecurityControlBlock() && 
                  !product.Frame.Reply.IsSecureChannelReply();
        }

        private static bool IsSecureChannelCommand(this Command command)
            => command == Command.CHLNG || command == Command.SCRYPT;

        private static bool IsSecureChannelReply(this Reply reply)
            => reply == Reply.CCRYPT || reply == Reply.RMAC_I;

        public static void SetInitialPayload(this IFrameProduct product)
        {
            if (product.IsCipher())
            {
                product.Payload.Cipher = product.Frame.Data;
            }
            else
            {
                product.Payload.Plain = product.Frame.Data;
            }
        }

        public static IRawTrace ToRawTrace(this IFrameProduct product)
        {
            return new RawTrace
            {
                Seconds = product.Timestamp.ToSeconds(),
                Nanoseconds = product.Timestamp.ToNanoseconds(),
                Io = "trace",
                Data = product.Frame.ToString(),
                TraceVersion = "1",
                Source = "osdpspy"
            };
        }

        private static long ToSeconds(this DateTime timestamp)
        {
            return (long)(timestamp - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private static long ToNanoseconds(this DateTime timestamp)
        {
            return 100 * (timestamp.Ticks % TimeSpan.TicksPerSecond);
        }

        private static string ToJson(this IRawTrace raw)
        {
            var trace = new StringBuilder("{");
            trace.Append($"\"timeSec\":\"{raw.Seconds}\",");
            trace.Append($"\"timeNano\":\"{raw.Nanoseconds:D09}\",");
            trace.Append($"\"io\":\"trace\",");
            trace.Append($"\"data\":\"{raw.Data.Trim()}\",");
            trace.Append($"\"osdpTraceVersion\":\"1\",");
            trace.Append($"\"osdpSource\":\"osdpspy\"");
            trace.Append("}");
            return trace.ToString();
        }

        public static string ToJson(this IFrameProduct product)
        {
            return product.ToRawTrace().ToJson();
        }

        public static string ToOsdpCaptureFileName(this DateTime timestamp)
        {
            var fileName = new StringBuilder();
            fileName.Append($"{timestamp.Year:D04}-");
            fileName.Append($"{timestamp.Month:D02}-");
            fileName.Append($"{timestamp.Day:D02}-");
            fileName.Append($"{timestamp.Hour:D02}");
            fileName.Append($"{timestamp.Minute:D02}");
            fileName.Append($"{timestamp.Second:D02}");
            fileName.Append(".osdpcap");
            return fileName.ToString();
        }
    }
}