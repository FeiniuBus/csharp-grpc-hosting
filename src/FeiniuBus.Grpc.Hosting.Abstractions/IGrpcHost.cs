using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeiniuBus.Grpc.Hosting
{
    public interface IGrpcHost : IDisposable
    {
        IServiceProvider Services { get; }

        void Start();

        Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}