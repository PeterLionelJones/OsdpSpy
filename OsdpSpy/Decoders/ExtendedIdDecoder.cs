using System;
using System.Collections.Generic;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public enum ExtendedIdTag
{
    Manufacturer = 0,
    ProductName = 1,
    SerialNumber = 2,
    FirmwareVersions = 3,
    HardwareDescription = 4
}

public class ExtendedIdDecoder : IMultipartMessageDecoder
{
    public Reply Reply => Reply.EXTID;

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
    public static IAnnotation Append(this IAnnotation output, KeyValuePair<ExtendedIdTag, string> item)
    {
        return item.Key switch
        {
            ExtendedIdTag.FirmwareVersions => output.AppendVersions(item),
            _ => output.AppendItem(item.Key.ToString(), item.Value)
        };
    }

    public static IAnnotation AppendVersions(this IAnnotation output, KeyValuePair<ExtendedIdTag, string> item)
    {
        var versions = item.Value.Split('\n');

        for (var i = 0; i < versions.Length; i++)
        {
            output.AppendItem($"FirmwareVersion{i+1}", versions[i]);
        }

        return output;
    }

    public static Dictionary<ExtendedIdTag, string> ToTlvStream(this byte[] input)
    {
        var items = new Dictionary<ExtendedIdTag, string>();
        var offset = 0;
        var remaining = input.Length;

        while (remaining > 0)
        {
            var tag = (ExtendedIdTag) input[offset];
            var itemLength = input[offset + 1] + (input[offset + 2] << 8);
            var length = itemLength + 3;
            var item = new byte[itemLength];
            Buffer.BlockCopy(input, offset + 3,  item, 0, itemLength);
            
            items.Add(tag, System.Text.Encoding.UTF8.GetString(item));
            
            remaining -= length;
            offset += length;
        }

        return items;
    }
}