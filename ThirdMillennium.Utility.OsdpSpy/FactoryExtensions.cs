using Microsoft.Extensions.DependencyInjection;

namespace ThirdMillennium.Utility.OSDP
{
    public static class FactoryExtensions
    {
        public static IServiceCollection AddFactories(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAnnotationsFactory, AnnotationsFactory>()
                .AddSingleton<IExchangeFactory, ExchangeFactory>()
                .AddSingleton<IFrameProductFactory, FrameProductFactory>()
                .AddTransient<IAnnotation, Annotation>();
        }
    }
}