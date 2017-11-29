using System;
using Grpc.Core;

namespace FeiniuBus.Grpc.Hosting
{
    public interface IApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; set; }
    }
}