using ThirdMillennium.Protocol;

namespace OsdpSpy.Listen
{
    internal static class BusFrameProducerExtensions
    {
        internal static int NextBaudRate(this int rate)
        {
            return rate switch
            {
                9600 => 230400,
                19200 => 9600,
                38400 => 19200,
                57600 => 38400,
                115200 => 57600,
                230400 => 115200,
                _ => 9600
            };
        }

        internal static bool Reopen(this IChannel channel, int rate)
        {
            var dev = channel.Device;
            channel.Close();
            return channel.Open(dev, rate);
        }
    }
}