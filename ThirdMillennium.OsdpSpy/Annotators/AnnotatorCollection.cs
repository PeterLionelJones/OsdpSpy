using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.OsdpSpy
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

        public override bool IncludeInput(IExchange input, IAnnotation annotation)
            => base.IncludeInput(input, annotation) || annotation.Contains("NonCompliance");
    }
}