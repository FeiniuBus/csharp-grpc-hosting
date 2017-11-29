using System;
using FeiniuBus.Grpc.Hosting.Internal;

namespace FeiniuBus.Grpc.Hosting.Builder
{
    public class ApplicationBuilderFactory : IApplicationBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IApplicationBuilder CreateBuilder()
        {
            return new ApplicationBuilder(_serviceProvider);
        }
    }
}