using System;
using Microsoft.Extensions.DependencyInjection;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class AnnotatorCollection : AnnotatorCollection<IExchange>
    {
        public AnnotatorCollection(IFactory<IAnnotation> factory, IServiceProvider provider) 
            : base(factory)
        {
            // Add the core annotators in the correct order.
            Add(provider.GetRequiredService<RawFrameAnnotator>());
            Add(provider.GetRequiredService<SecureChannelAnnotator>());
            Add(provider.GetRequiredService<CommandAnnotator>());
            Add(provider.GetRequiredService<ReplyAnnotator>());

            // Add the other annotators found in the assembly.
            AddRange(provider.GetExtensionAnnotators<Annotator<IExchange>>());
        }
    }
}