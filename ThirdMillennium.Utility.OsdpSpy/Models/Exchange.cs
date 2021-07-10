using System;

namespace ThirdMillennium.Utility.OSDP
{
    public class Exchange : IExchange
    {
        public long Sequence { get; set; }
        public IFrameProduct Acu { get; set; }
        public IFrameProduct Pd { get; private set; }

        public void AddReceived(IFrameProduct rx)
        {
            // Cannot set the received frame twice!
            if (Pd != null)
                throw new ArgumentException(
                    "The received frame for this Exchange has already been set");

            // Set the received frame.
            Pd = rx;
        }
    }
}