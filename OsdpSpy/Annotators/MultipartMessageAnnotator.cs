using System;
using System.Linq;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Decoders;
using OsdpSpy.Osdp;

namespace OsdpSpy.Annotators;

public class MultipartMessageAnnotator(IMultipartMessageDecoderCollection decoder, IFactory<IAnnotation> factory) 
    : AlertingAnnotator<IExchange>(factory)
{
    private readonly MultipartMessageReaders _readers = new();
    
    public override void Annotate(IExchange input, IAnnotation output)
    {
        if (input.Pd.Frame == null) return;
        if (!input.Pd.Frame.Reply.IsMultipartMessage()) return;
        if (input.Pd.Payload.Plain == null) return;

        var payload = input.Pd.Payload.Plain;

        var reader = new MultipartMessageFragment
        {
            Address = input.Pd.Frame.Address,
            Reply = input.Pd.Frame.Reply,
            TotalSize = payload.GetMultipartMessageSize(),
            Offset = payload.GetMultipartMessageOffset(),
            Fragment = payload.GetMultipartMessageFragment()
        };
        
        var complete = _readers.AddFragment(reader, out var data);

        if (complete)
        {
            decoder.Decode(reader.Reply, data, output);
        }
    }
}

public static class MultipartMessageExtensions
{
    public static bool IsMultipartMessage(this Reply reply)
    {
        return reply switch
        {
            Reply.EXT_PDID => true,
            _ => false 
        };
    }
    
    internal static int GetMultipartMessageSize(this byte[] input) 
        => input[0] | (input[1] << 8);

    internal static int GetMultipartMessageOffset(this byte[] input) 
        => input[2] | (input[3] << 8);

    private static int GetMultipartMessageFragmentSize(this byte[] input) 
        => input[4] | (input[5] << 8);

    internal static byte[] GetMultipartMessageFragment(this byte[] input)
    {
        var fragment = new byte[input.GetMultipartMessageFragmentSize()];
        Buffer.BlockCopy(input, 6, fragment, 0, fragment.Length);
        return fragment;
    }
}

public class MultipartMessageFragment
{
    public int Address { get; init; }
    public Reply Reply { get; init; }
    public int TotalSize { get; init; }
    public int Offset { get; init; }
    public byte[] Fragment { get; init; }
}

public class MultipartMessageReaders
{
    private readonly System.Collections.Generic.List<MultipartMessageReader> _readers = [];

    public bool AddFragment(MultipartMessageFragment fragment, out byte[] data)
    {
        data = null;
        
        var reader = _readers.FirstOrDefault(r => r.Address == fragment.Address);
        
        if (reader == null || reader.Reply != fragment.Reply || fragment.Offset == 0)
        {
            reader = new MultipartMessageReader
            {
                Address = fragment.Address,
                Reply = fragment.Reply,
                Data = new byte[fragment.TotalSize]
            };
            
            _readers.Add(reader);
        }

        Buffer.BlockCopy(
            fragment.Fragment, 
            0, 
            reader.Data, 
            fragment.Offset, 
            fragment.Fragment.Length);

        var complete =  fragment.Offset + fragment.Fragment.Length == fragment.TotalSize;

        if (complete)
        {
            data = reader.Data;
        }

        return complete;
    }
}

public class MultipartMessageReader
{
    public int Address { get; init; }
    public Reply Reply { get; init; }
    public byte[] Data { get; init; }
}