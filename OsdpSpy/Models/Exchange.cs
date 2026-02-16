using System;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Models;

public class Exchange : IExchange
{
    private Exchange() {}

    public static IExchange Create(long sequence, IFrameProduct acu)
    {
        return new Exchange
        {
            Sequence = sequence,
            Acu = acu
        };
    }
        
    public long Sequence { get; private init; }
    public IFrameProduct Acu { get; private init; }
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