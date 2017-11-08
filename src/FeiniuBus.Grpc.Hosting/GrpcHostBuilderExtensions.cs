using System;
using System.Reflection;
using FeiniuBus.Grpc.Hosting.Internal;
using FeiniuBus.Grpc.Hosting.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public static class GrpcHostBuilderExtensions
    {
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