using System;
using System.Collections.ObjectModel;
using System.Linq;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public class MultipartMessageDecoderCollection : Collection<IMultipartMessageDecoder>, IMultipartMessageDecoderCollection
{
    public MultipartMessageDecoderCollection(IServiceProvider provider)
    {
        var decoders = provider.GetImplementationsOf<IMultipartMessageDecoder>();
            
        foreach (var decoder in decoders)
        {
            Add(decoder);
        }
    }
    
    public void Decode(Reply reply, byte[] input, IAnnotation output)
        => this.FirstOrDefault(x => x.Reply == reply)?.Decode(input, output);
}