using System;

namespace OsdpSpy.Extensions;

public static class ToHexStringExtensions
{
    public static string ToHexString(this byte[] data)
        => BitConverter.ToString(data).Replace('-', ' ');
        
    public static string ToHexString(this byte[] data, int start)
        => BitConverter.ToString(data, start).Replace('-', ' ');
        
    public static string ToHexString(this byte[] data, int start, int length)
        => BitConverter.ToString(data, start, length).Replace('-', ' ');
}