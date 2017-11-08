using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class StartupMethods
    {
        public StartupMethods(object instance, Func<IServiceCollection, IServiceProvider> configureServices)
        {
            Debug.Assert(configureServices != null);
            StartupInstance = instance;
            ConfigureServicesDelegate = configureServices;
        }
        
        public object StartupInstance { get; }
        
        public Func<IServiceCollection, IServiceProvider> ConfigureServicesDelegate { get; }
    }
}