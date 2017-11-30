using System.Collections.Generic;
using FeiniuBus.Grpc.Hosting.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeiniuBus.Grpc.Hosting.Tests
{
    public class GrpcHostTests
    {
        [Fact]
        public void EnvDefaultsToProductionIfNoConfig()
        {
            using (var host = new GrpcHostBuilder().UseStartup<Fakes.Startup>().Build())
            {
                var env = host.Services.GetService<IHostingEnvironment>();
                Assert.Equal(EnvironmentName.Production, env.EnvironmentName);
            }
        }

        [Fact]
        public void IsEnvironment_Extension_Is_Case_Insensitive()
        {
            using (var host = new GrpcHostBuilder().UseStartup<Fakes.Startup>().Build())
            {
                host.Start();
                var env = host.Services.GetService<IHostingEnvironment>();
                Assert.True(env.IsEnvironment(EnvironmentName.Production));
                Assert.True(env.IsEnvironment("producTion"));
            }
        }

        [Fact]
        public void EnvDefaultsToConfigValueIfSpecified()
        {
            var vals = new Dictionary<string, string>
            {
                {"Environment", EnvironmentName.Staging}
            };

            var builder = new ConfigurationBuilder().AddInMemoryCollection(vals);
            var config = builder.Build();

            using (var host = new GrpcHostBuilder().UseConfiguration(config).UseStartup<Fakes.Startup>().Build())
            {
                var env = host.Services.GetService<IHostingEnvironment>();
                Assert.Equal(EnvironmentName.Staging, env.EnvironmentName);
            }
        }

        [Fact]
        public void WebHost_InvokesConfigureMethodsOnlyOnce()
        {
            using (var host = new GrpcHostBuilder().UseStartup<CountStartup>().Build())
            {
                host.Start();
                Assert.Equal(1, CountStartup.ConfigureServicesCount);
                Assert.Equal(1, CountStartup.ConfigureCount);
            }
        }
    }
}