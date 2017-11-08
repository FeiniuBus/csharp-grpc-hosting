using System;
using FeiniuBus.Grpc.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public class GrpcHostBuilder : IGrpcHostBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private bool _grpcHostBuilt;

        public GrpcHostBuilder()
        {
            _hostingEnvironment = new HostingEnvironment();
        }
        
        public IGrpcHost Build()
        {
            if (_grpcHostBuilt)
            {
                throw new InvalidOperationException("GrpcHostBuilder allows creation only of a single instance of GrpcHost");
            }
            _grpcHostBuilt = true;
            
            throw new NotImplementedException();
        }

        public IGrpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            throw new NotImplementedException();
        }

        public IGrpcHostBuilder UseSetting(string key, string value)
        {
            throw new NotImplementedException();
        }

        private IServiceCollection BuildCommonServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton(_hostingEnvironment);
            services.AddLogging();
            
            return services;
        }
    }
}