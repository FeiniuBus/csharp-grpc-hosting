using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class GrpcHost : IGrpcHost
    {
        private ILogger<GrpcHost> _logger;
        private readonly IServiceProvider _hostingServiceProvider;
        private IServiceProvider _applicationServices;
        private readonly IServiceCollection _applicationServiceCollection;
        private IStartup _startup;
        private Server Server { get; set; }

        public GrpcHost(IServiceCollection appServices, IServiceProvider hostingServiceProvider)
        {
            if (appServices == null)
            {
                throw new ArgumentNullException(nameof(appServices));
            }
            if (hostingServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(hostingServiceProvider));
            }

            _hostingServiceProvider = hostingServiceProvider;
            _applicationServiceCollection = appServices;
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IServiceProvider Services
        {
            get
            {
                EnsureApplicationServices();
                return _applicationServices;
            }
        }
        
        public void Start()
        {
            _logger = _applicationServices.GetRequiredService<ILogger<GrpcHost>>();
            _logger.Starting();
            
            Server.Start();
            
            _logger.Started();
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _hostingServiceProvider.GetRequiredService<IStartup>();
        }

        private void EnsureServer()
        {
            if (Server == null)
            {
                var rpcs = _applicationServices.GetServices<IGrpcService>();
                Server = new Server();
            }
        }        
    }
}