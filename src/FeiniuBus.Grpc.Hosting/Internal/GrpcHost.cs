using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class GrpcHost : IGrpcHost
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IServiceProvider Services { get; }
        public void Start()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}