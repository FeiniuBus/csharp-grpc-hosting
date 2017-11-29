using System;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public interface IStartup
    {
        IServiceProvider ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder app);
    }
}