using System;
using System.Collections.ObjectModel;
using System.Linq;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class CommandDecoderCollection : Collection<ICommandDecoder>, ICommandDecoderCollection
    {
        public CommandDecoderCollection(IServiceProvider provider)
        {
            var decoders = provider
                .GetImplementationsOf<ICommandDecoder>();
            
            foreach (var decoder in decoders)
            {
                Add(decoder);
            }
        }
        
        public void Decode(Command command, byte[] input, IAnnotation output)
            => this.FirstOrDefault(x => x.Command == command)?.Decode(input, output);
    }
}