using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly:InternalsVisibleTo("OsdpSpy.Tests.Annotations")]

namespace OsdpSpy.Annotations;

public class Factory<T> : IFactory<T>
{
    internal Factory(IServiceProvider services) => _services = services;
    private readonly IServiceProvider _services;
    public T Create() => _services.GetRequiredService<T>();
}