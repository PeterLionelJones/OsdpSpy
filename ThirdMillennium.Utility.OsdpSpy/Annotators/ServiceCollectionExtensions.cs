using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnnotators(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAnnotatorCollection<IExchange>, AnnotatorCollection>()
                .AddAnnotators<ExchangeAnnotator>();
        }

        private static IServiceCollection AddAnnotators<T>(this IServiceCollection services)
        {
            foreach (var annotatorType in GetAnnotatorTypes<T>())
            {
                services.AddSingleton(annotatorType);
            }

            return services;
        }

        public static IEnumerable<T> GetAnnotators<T>(this IServiceProvider provider)
        {
            return GetAnnotatorTypes<T>()
                .Select(provider.GetService)
                .Cast<T>();
        }

        private static IEnumerable<Type> GetAnnotatorTypes<T>()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(T)) && t.IsPublic);
        }
    }
}