using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public static class RawTraceExtensions
    {
        public static IFrameProduct ToFrameProduct(this IRawTrace raw)
        {
            var frame = raw.ToFrame();
            return frame == null ? null : new FrameProduct(raw.ToTimestamp(), frame);
        }

        private static DateTime ToTimestamp(this IRawTrace raw)
        {
            // Build the timestamp for this frame.
            var seconds =
                DateTime.UnixEpoch +
                TimeSpan.FromSeconds(raw.Seconds);

            // Done!
            return new DateTime(seconds.Ticks + raw.Nanoseconds / 100);
        }

        private static Frame ToFrame(this IRawTrace raw)
        {
            // Convert the raw data to a byte array and attempt to build a frame.
            var rx = new ResponseFrame();
            var hex = raw.Data.Trim().Split(' ');
            foreach (var b in hex)
            {
                var complete = rx.AddByte(Convert.ToByte(b, 16));
                if (complete)
                {
                    rx.Disassemble();
                    return rx;
                }
            }

            // There is no valid frame in the data.
            return null;
        }
    }
}