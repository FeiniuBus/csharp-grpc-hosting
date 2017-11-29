using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class StartupMethods
    {
        public StartupMethods(object instance, Action<IApplicationBuilder> configure,
            Func<IServiceCollection, IServiceProvider> configureServices)
        {
            Debug.Assert(configureServices != null);
            Debug.Assert(configure != null);
            
            StartupInstance = instance;
            ConfigureDelegate = configure;
            ConfigureServicesDelegate = configureServices;
        }

        public object StartupInstance { get; }
        
        public Func<IServiceCollection, IServiceProvider> ConfigureServicesDelegate { get; }
        
        public Action<IApplicationBuilder> ConfigureDelegate { get; }
    }
}