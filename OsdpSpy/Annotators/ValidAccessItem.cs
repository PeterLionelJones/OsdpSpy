using System;
using System.Text;
using OsdpSpy.Decoders;

namespace OsdpSpy.Annotators;

public class ValidAccessItem
{
    public ValidAccessItem(bool isCard, byte[] payload)
    {
        Timestamp = DateTime.UtcNow;
        IsCard = isCard;
        Payload = payload;
    }

    public DateTime Timestamp { get; }
    public bool IsCard { get; }
    public byte[] Payload { get; }

    public override string ToString()
    {
        if (IsCard)
        {
            return $"Card: {Payload.ToRawCardString()}";
        }
            
        var builder = new StringBuilder();
        foreach (var key in Payload.ToKeyArray())
        {
            builder.Append($"{key} ");
        }
        return builder.ToString();
    }
}