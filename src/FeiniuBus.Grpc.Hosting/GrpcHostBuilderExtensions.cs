using System;
using System.Reflection;
using FeiniuBus.Grpc.Hosting.Internal;
using FeiniuBus.Grpc.Hosting.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.Grpc.Hosting
{
    public static class GrpcHostBuilderExtensions
    {
        public static IGrpcHostBuilder UseDefaultServiceProvider(this IGrpcHostBuilder hostBuilder,
            Action<GrpcHostBuilderContext, ServiceProviderOptions> configure)
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {
                var options = new ServiceProviderOptions();
                configure(context, options);
                services.Replace(
                    ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(
                        new DefaultServiceProviderFactory(options)));
            });
        }
        
        public static IGrpcHostBuilder ConfigureAppConfiguration(this IGrpcHostBuilder hostBuilder,
            Action<IConfigurationBuilder> configureDelegate)
        {
            return hostBuilder.ConfigureAppConfiguration((context, builder) => configureDelegate(builder));
        }

        public static IGrpcHostBuilder ConfigureLogging(this IGrpcHostBuilder hostBuilder,
            Action<ILoggingBuilder> configureLogging)
        {
            return hostBuilder.ConfigureServices(collection => collection.AddLogging(configureLogging));
        }

        public static IGrpcHostBuilder ConfigureLogging(this IGrpcHostBuilder hostBuilder,
            Action<GrpcHostBuilderContext, ILoggingBuilder> configureLogging)
        {
            return hostBuilder.ConfigureServices((context, collection) =>
                collection.AddLogging(builder => configureLogging(context, builder)));
        }
        
        public static IGrpcHostBuilder UseStartup(this IGrpcHostBuilder builder, Type startupType)
        {
            return builder.ConfigureServices(services =>
            {
                if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                {
                    services.AddSingleton(typeof(IStartup), startupType);
                }
                else
                {
                    services.AddSingleton(typeof(IStartup), sp =>
                    {
                        var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                        return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType,
                            hostingEnvironment.EnvironmentName));
                    });
                }
            });
        }

        public static IGrpcHostBuilder UseStartup<TStartup>(this IGrpcHostBuilder builder) where TStartup : class
        {
            return builder.UseStartup(typeof(TStartup));
        }
    }
}