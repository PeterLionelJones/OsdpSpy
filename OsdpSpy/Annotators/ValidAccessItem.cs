using System;
using System.Text;
using OsdpSpy.Decoders;

namespace OsdpSpy.Annotators;

public class ValidAccessItem(bool isCard, byte[] payload)
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public bool IsCard { get; } = isCard;
    public byte[] Payload { get; } = payload;

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