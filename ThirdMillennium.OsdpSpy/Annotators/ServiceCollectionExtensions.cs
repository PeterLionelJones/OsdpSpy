using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OsdpSpy.Annotations;

namespace ThirdMillennium.OsdpSpy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnnotators(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAnnotatorCollection<IExchange>, AnnotatorCollection>()
                .AddAnnotators<Annotator<IExchange>>()
                .AddSingleton<IReplyDecoderCollection, ReplyDecoderCollection>()
                .AddSingleton<ICommandDecoderCollection, CommandDecoderCollection>()
                .AddSingleton<ISecureChannelSink, ReaderAlertAnnotator>()
                .AddImplementationsOf<IDecoder>();
        }

        private static IServiceCollection AddAnnotators<T>(this IServiceCollection services)
        {
            var annotatorTypes = GetSubclassesOf<T>()
                .Where(t => t != typeof(AlertingAnnotator<IExchange>));
            
            foreach (var annotatorType in annotatorTypes)
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

        public static IEnumerable<T> GetExtensionAnnotators<T>(this IServiceProvider provider)
        {
            return GetSubclassesOf<T>()
                .Where(t => 
                    t != typeof(AlertingAnnotator<IExchange>) &&
                    t != typeof(RawFrameAnnotator) &&
                    t != typeof(SecureChannelAnnotator) &&
                    t != typeof(CommandAnnotator) &&
                    t != typeof(ReplyAnnotator))
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

        private static IEnumerable<Type> GetImplementationsOf<T>()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass);
        }

        public static IAnnotation CreateOsdpAlert(
            this AlertingAnnotator<IExchange> annotator,
            string alertMessage)
        {
            return annotator.CreateAlert(
                "OsdpAlert", alertMessage, 
                "OSDP Alert");
        }
    }
}