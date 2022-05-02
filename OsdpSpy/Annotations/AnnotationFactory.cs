using System;

namespace OsdpSpy.Annotations
{
    public class AnnotationFactory : Factory<IAnnotation>
    {
        public AnnotationFactory(IServiceProvider services) : base(services) {}
    }
}