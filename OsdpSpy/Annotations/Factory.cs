using System;
using Microsoft.Extensions.DependencyInjection;

namespace OsdpSpy.Annotations
{
    public class Factory<T> : IFactory<T>
    {
        protected Factory(IServiceProvider services) => _services = services;
        private readonly IServiceProvider _services;
        public T Create() => _services.GetRequiredService<T>();
    }
}