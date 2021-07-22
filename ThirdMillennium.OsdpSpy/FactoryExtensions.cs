using Microsoft.Extensions.DependencyInjection;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.OsdpSpy
{
    public static class FactoryExtensions
    {
        public static IServiceCollection AddFactories(this IServiceCollection services)
        {
            return services
                .AddSingleton<IFactory<IAnnotation>, AnnotationFactory>()
                .AddSingleton<IExchangeFactory, ExchangeFactory>()
                .AddSingleton<IFrameProductFactory, FrameProductFactory>()
                .AddTransient<IAnnotation, Annotation>();
        }
    }
}