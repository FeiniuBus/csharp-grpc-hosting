using System.Threading;

namespace FeiniuBus.Grpc.Hosting
{
    public interface IApplicationLifetime
    {
        CancellationToken ApplicationStarted { get; }
        
        CancellationToken ApplicationStopping { get; }
        
        CancellationToken ApplicationStopped { get; }

        void StopApplication();
    }
}