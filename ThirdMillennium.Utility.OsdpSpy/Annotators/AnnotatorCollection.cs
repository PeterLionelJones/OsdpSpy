using System;
using System.Linq;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class AnnotatorCollection : AnnotatorCollection<IExchange>
    {
        public AnnotatorCollection(IFactory<IAnnotation> factory, IServiceProvider provider) 
            : base(factory)
        {
            AddRange(provider
                .GetAnnotators<ExchangeAnnotator>()
                .OrderBy(x => x.Priority));
        }
    }
}