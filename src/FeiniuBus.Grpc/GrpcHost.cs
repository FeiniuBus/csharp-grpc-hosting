using System.IO;
using FeiniuBus.Grpc.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.Grpc
{
    public static class GrpcHost
    {
        public static IGrpcHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new GrpcHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((context, logging) =>
                    {
                        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                    })
                .UseDefaultServiceProvider((context, options) =>
                    {
                        options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                    });

            return builder;
        }
    }
}