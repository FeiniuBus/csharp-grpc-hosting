using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;

namespace FeiniuBus.Grpc.Hosting.Consul
{
    public class ConsulGrpcHost
    {
        private readonly IGrpcHost _host;
        private readonly string _consulAddress;
        private readonly int _consulPort;
        private readonly string _datacenter;
        private ServiceDiscovery _serviceDiscovery;
        private string _serviceId;

        public ConsulGrpcHost(IGrpcHost host, string consulAddress) : this(host, consulAddress, 8500)
        {
        }

        public ConsulGrpcHost(IGrpcHost host, string consulAddress = "localhost", int consulPort = 8500, string datacenter = "dc1")
        {
            _host = host;
            _consulAddress = consulAddress;
            _consulPort = consulPort;
            _datacenter = datacenter;
        }

        public async Task StartAsync(string name, string address, int port, CancellationToken token = default (CancellationToken))
        {
            _host.Start();

            var builder = new UriBuilder("http", _consulAddress, _consulPort);
            _serviceDiscovery = new ServiceDiscovery(builder.Uri, _datacenter);

            if (IPAddress.TryParse(address, out var ip))
            {
                _serviceId = await _serviceDiscovery.RegisterServiceAsync(name, new IPEndPoint(ip, port), token)
                    .ConfigureAwait(false);
            }
            else
            {
                _serviceId = await _serviceDiscovery.RegisterServiceAsync(name, new DnsEndPoint(address, port), token)
                    .ConfigureAwait(false);
            }
        }

        public async Task StopAsync(CancellationToken token = default (CancellationToken))
        {
            await _serviceDiscovery.UnregisterServiceAsync(_serviceId, token).ConfigureAwait(false);
            _host?.Dispose();
        }
    }
}