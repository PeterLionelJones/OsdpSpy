using System;
using System.Collections.Generic;
using System.Linq;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public enum ExtendedIdTag
{
    Manufacturer = 0,
    ProductName = 1,
    SerialNumber = 2,
    FirmwareVersion = 3,
    HardwareDescription = 4,
    Url = 5,
    ConfigurationReference = 6,
    Unknown = 0xFF
}

public class ExtendedIdEntry(ExtendedIdTag tag, string value)
{
    public ExtendedIdTag Tag { get; init; } = tag;
    public int Index { get; set; }
    public string Value { get; init; } = value;
}

public class ExtendedIdDecoder : IMultipartMessageDecoder
{
    public Reply Reply => Reply.EXT_PDID;

    public void Decode(byte[] input, IAnnotation output)
    {
        output.AppendNewLine();
        
        var items = input.ToTlvStream();

        foreach (var item in items)
        {
            output.Append(item);
        }
    }
}

internal static class ExtendedIdDecoderExtensions
{
    public static IAnnotation Append(this IAnnotation output, ExtendedIdEntry item) 
        => output.AppendItem(item.ToItemName(), item.Value);

    private static string ToItemName(this ExtendedIdEntry item) 
        => item.Index == 0 ? item.Tag.ToString() : $"{item.Tag}{item.Index + 1}";

    public static List<ExtendedIdEntry> ToTlvStream(this byte[] input)
    {
        var items = new List<ExtendedIdEntry>();
        var offset = 0;
        var remaining = input.Length;

        while (remaining > 0)
        {
            var tag = (ExtendedIdTag) input[offset];
            var itemLength = input[offset + 1] + (input[offset + 2] << 8);
            var length = itemLength + 3;
            var item = new byte[itemLength];
            Buffer.BlockCopy(input, offset + 3,  item, 0, itemLength);
            
            items.Add(new ExtendedIdEntry(tag, System.Text.Encoding.UTF8.GetString(item)));
            
            remaining -= length;
            offset += length;
        }

        return items.SortAndNumber();
    }

    private static List<ExtendedIdEntry> SortAndNumber(this List<ExtendedIdEntry> input)
    {
        var output = input.OrderBy(x => x.Tag).ToList();

        var lastTag = ExtendedIdTag.Unknown;
        var index = 0;

        foreach (var entry in output)
        {
            if (entry.Tag != lastTag)
            {
                lastTag = entry.Tag;
                index = 0;
            }
            entry.Index = index++;
        }

        return output;
    }
}