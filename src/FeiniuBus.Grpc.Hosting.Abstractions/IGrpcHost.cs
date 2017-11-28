using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace FeiniuBus.Grpc.Hosting
{
    public interface IGrpcHost : IDisposable
    {
        IServiceProvider Services { get; }
        
        Server Server { get; }

        void Start();

        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}