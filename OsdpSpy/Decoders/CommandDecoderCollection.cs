using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public class CommandDecoderCollection : Collection<ICommandDecoder>, ICommandDecoderCollection
{
    public CommandDecoderCollection(IServiceProvider provider)
    {
        var decoders = provider
            .GetImplementationsOf<ICommandDecoder>();
            
        foreach (var decoder in decoders)
        {
            Add(decoder);
        }
    }
        
    public void Decode(Command command, byte[] input, IAnnotation output)
        => this.FirstOrDefault(x => x.Command == command)?.Decode(input, output);
}

internal static class CommandDecoderCollectionExtensions
{
    public static IEnumerable<T> GetImplementationsOf<T>(this IServiceProvider provider)
    {
        return Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass)
            .Select(provider.GetService)
            .Cast<T>();
    }
}