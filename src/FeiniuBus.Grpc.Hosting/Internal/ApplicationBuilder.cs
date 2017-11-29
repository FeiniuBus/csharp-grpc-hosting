using System;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class ApplicationBuilder : IApplicationBuilder
    {
        public ApplicationBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
        }
        
        public IServiceProvider ApplicationServices { get; set; }
    }
}