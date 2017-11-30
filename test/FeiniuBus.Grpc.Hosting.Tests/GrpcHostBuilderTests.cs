using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeiniuBus.Grpc.Hosting.Tests
{
    public class GrpcHostBuilderTests
    {
        [Fact]
        public void ApplicationLifetimeRegistered()
        {
            var builder = CreateGrpcHostBuilder();
            var host = builder.Build();
            using (host)
            {
                host.Start();
                var services = host.Services.GetServices<IApplicationLifetime>();
                Assert.NotNull(services);
                Assert.NotEmpty(services);
            }
        }

        [Fact]
        public void HostConfigurationBuilde()
        {
            var settings = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var builder = CreateGrpcHostBuilder().UseConfiguration(config);
            var host = builder.Build();
            using (host)
            {
                host.Start();
                var configuration = host.Services.GetRequiredService<IConfiguration>();
                Assert.NotNull(configuration);
                Assert.Equal("value1", configuration["key1"]);
                Assert.Equal("value2", configuration["key2"]);
            }
        }

        [Fact]
        public void HostEndpointsRegistered()
        {
            var builder = CreateGrpcHostBuilder().UseUrls("0.0.0.0:4099");
            var host = builder.Build();
            using (host)
            {
                host.Start();
                Assert.NotEmpty(host.Server.Ports);
                Assert.Equal("0.0.0.0", host.Server.Ports.First().Host);
                Assert.Equal(4099, host.Server.Ports.First().Port);
            }
        }
        
        private IGrpcHostBuilder CreateGrpcHostBuilder()
        {
            return new GrpcHostBuilder().UseStartup<Fakes.Startup>();
        }
    }
}