using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Tests.Fakes
{
    public class CountStartup
    {
        public static int ConfigureServicesCount;
        public static int ConfigureCount;

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesCount++;
        }

        public void Configure(IApplicationBuilder builder)
        {
            ConfigureCount++;
        }
    }
}