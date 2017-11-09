using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class GrpcHost : IGrpcHost
    {
        private const string DefaultHost = "localhost";
        private const int DefaultPort = 4009;
        
        private ILogger<GrpcHost> _logger;
        private readonly IServiceProvider _hostingServiceProvider;
        private readonly List<Type> _serviceTypes;
        private readonly IConfiguration _config;
        private IServiceProvider _applicationServices;
        private readonly IServiceCollection _applicationServiceCollection;
        private IStartup _startup;
        private bool _stopped;
        private Server Server { get; set; }

        public GrpcHost(IServiceCollection appServices, IServiceProvider hostingServiceProvider, IConfiguration config,
            List<Type> serviceTypes)
        {
            _hostingServiceProvider = hostingServiceProvider ?? throw new ArgumentNullException(nameof(hostingServiceProvider));
            _applicationServiceCollection = appServices ?? throw new ArgumentNullException(nameof(appServices));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _serviceTypes = serviceTypes ?? throw new ArgumentNullException(nameof(serviceTypes));
        }

        public void Dispose()
        {
            if (!_stopped)
            {
                try
                {
                    StopAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    _logger?.ServerShutdownException(e);
                }
            }
            
            (_applicationServices as IDisposable)?.Dispose();
            (_hostingServiceProvider as IDisposable)?.Dispose();
        }

        public IServiceProvider Services
        {
            get
            {
                EnsureApplicationServices();
                return _applicationServices;
            }
        }

        public void Initialize()
        {
            EnsureApplicationServices();
            EnsureServer();
        }
        
        public void Start()
        {
            _logger = _applicationServices.GetRequiredService<ILogger<GrpcHost>>();
            _logger.Starting();
            
            Initialize();
            Server.Start();
            
            _logger.Started();
        }

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_stopped)
            {
                return;
            }
            _stopped = true;
            
            _logger?.Shutdown();

            if (Server != null)
            {
                await Server.ShutdownAsync().ConfigureAwait(false);
            }
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
                Server = new Server();
                
                var urls = _config[GrpcHostDefaults.ServerUrlsKey];
                if (string.IsNullOrEmpty(urls))
                {
                    Server.Ports.Add(new ServerPort(DefaultHost, DefaultPort, ServerCredentials.Insecure));
                }
                else
                {
                    foreach (var value in urls.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var parts = value.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 1)
                        {
                            Server.Ports.Add(new ServerPort(parts[0], DefaultPort, ServerCredentials.Insecure));
                        }
                        else
                        {
                            Server.Ports.Add(new ServerPort(parts[0], Convert.ToInt32(parts[1]),
                                ServerCredentials.Insecure));
                        }
                    }
                }

                foreach (var serviceType in _serviceTypes)
                {
                    ServerServiceDefinition definition =
                        RpcServcieLoader.LoadService(_applicationServices, serviceType);

                    if (definition == null)
                    {
                        _logger.LogWarning(LoggerEventIds.ServiceDefinitionNull, "Service type: {0}'s definition is null", serviceType);
                        continue;
                    }
                    
                    Server.Services.Add(definition);
                }
            }
        }        
    }
}