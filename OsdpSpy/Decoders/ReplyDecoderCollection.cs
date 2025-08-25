using System;
using System.Collections.ObjectModel;
using System.Linq;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public class ReplyDecoderCollection : Collection<IReplyDecoder>, IReplyDecoderCollection
{
    public ReplyDecoderCollection(IServiceProvider provider)
    {
        var decoders = provider.GetImplementationsOf<IReplyDecoder>();
            
        foreach (var decoder in decoders)
        {
            Add(decoder);
        }
    }
        
    public void Decode(Reply reply, byte[] input, IAnnotation output)
        => this.FirstOrDefault(x => x.Reply == reply)?.Decode(input, output);
}