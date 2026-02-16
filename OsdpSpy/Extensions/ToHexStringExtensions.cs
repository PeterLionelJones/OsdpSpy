using System;
using System.Diagnostics;
using System.Text;
using OsdpSpy.Annotations;

namespace OsdpSpy.Extensions;

public static class ToHexStringExtensions
{
    private static System.Collections.Generic.List<byte[]> Slice(this byte[] data, int start, int length, int width)
    {
        if (length > width)
        {
            Debug.WriteLine("length > width");
        }
        
        var offset = start;
        var remaining = length;
        var result = new System.Collections.Generic.List<byte[]>();

        while (remaining > 0)
        {
            var fragmentLength = width > remaining ? remaining : width;
            
            var block = new byte[fragmentLength];
            Buffer.BlockCopy(data, offset, block, 0, fragmentLength);
            
            result.Add(block);
            
            remaining -= fragmentLength;
            offset += fragmentLength;
        }

        return result;
    }
    
    private static System.Collections.Generic.List<byte[]> Slice(this byte[] data, int start, int width)
        => Slice(data, width, data.Length - start, width);

    private static System.Collections.Generic.List<byte[]> Slice(this byte[] data, int width)
        => Slice(data, 0,  data.Length, width);
    
    private static string ToAnnotatedHexBlock(this System.Collections.Generic.List<byte[]> slices)
    {
        var blankColumn = $"\n{" ",Annotator.FirstColumnWidth - 5}";
        var builder = new StringBuilder();
        var pad = false;

        foreach (var slice in slices)
        {
            if (pad) builder.Append(blankColumn);
            builder.Append(BitConverter.ToString(slice).Replace('-', ' '));
            pad = true;
        }
        
        return builder.ToString();
    }
    
    public static string ToHexString(this byte[] data) 
        => data.Slice(Annotator.HexBlockBytes).ToAnnotatedHexBlock();

    public static string ToHexString(this byte[] data, int start)
        => data.Slice(start, Annotator.HexBlockBytes).ToAnnotatedHexBlock();
        
    public static string ToHexString(this byte[] data, int start, int length)
        => data.Slice(start, length, Annotator.HexBlockBytes).ToAnnotatedHexBlock();
}