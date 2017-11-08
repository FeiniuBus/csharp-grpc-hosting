using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public static class GrpcHostBuilderExtensions
    {
        public static IGrpcHostBuilder UseStartup(this IGrpcHostBuilder builder, Type startupType)
        {
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;
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
                        return null;
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