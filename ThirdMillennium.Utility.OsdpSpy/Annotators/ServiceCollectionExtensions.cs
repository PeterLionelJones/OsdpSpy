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
                .AddAnnotators<ExchangeAnnotator>()
                .AddSingleton<IReplyDecoderCollection, ReplyDecoderCollection>()
                .AddSingleton<ICommandDecoderCollection, CommandDecoderCollection>()
                .AddImplementationsOf<IDecoder>();
        }

        private static IServiceCollection AddAnnotators<T>(this IServiceCollection services)
        {
            foreach (var annotatorType in GetSubclassesOf<T>())
            {
                services.AddSingleton(annotatorType);
            }

            return services;
        }

        private static IServiceCollection AddImplementationsOf<T>(this IServiceCollection services)
        {
            foreach (var replyDecoder in GetImplementationsOf<T>())
            {
                services.AddSingleton(replyDecoder);
            }

            return services;
        }

        public static IEnumerable<T> GetAnnotators<T>(this IServiceProvider provider)
        {
            return GetSubclassesOf<T>()
                .Select(provider.GetService)
                .Cast<T>();
        }

        private static IEnumerable<Type> GetSubclassesOf<T>()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(T)) && t.IsPublic);
        }

        public static IEnumerable<Type> GetImplementationsOf<T>()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass);
        }
    }
}