using Microsoft.Extensions.DependencyInjection;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;

namespace OsdpSpy;

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