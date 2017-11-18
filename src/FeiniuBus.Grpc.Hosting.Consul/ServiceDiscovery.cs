using System;
using System.Net;
using System.Threading.Tasks;
using Consul;

namespace FeiniuBus.Grpc.Hosting.Consul
{
    internal sealed class ServiceDiscovery
    {
        private readonly IConsulClient _client;

        public ServiceDiscovery(Uri endpoint, string datacenter)
        {
            _client = new ConsulClient(c =>
            {
                c.Address = endpoint;
                c.Datacenter = datacenter;
            });
        }

        public async Task<Entry> RegisterServiceAsync(string name, EndPoint endpoint)
        {
            if (endpoint is IPEndPoint node)
            {
                var serviceId = string.Format("{0}-{1}-{2}", name, node.Address.ToString(), node.Port);
                var checkId = string.Format("{0}-{1}", node.Address.ToString(), node.Port);
                
                var acr = new AgentCheckRegistration
                {
                    TCP = node.ToString(),
                    Name = checkId,
                    ID = checkId,
                    Interval = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10)
                };
                
                var asr = new AgentServiceRegistration
                {
                    Address = node.Address.ToString(),
                    ID = serviceId,
                    Name = name,
                    Port = node.Port,
                    Check = acr
                };

                var res = await _client.Agent.ServiceRegister(asr).ConfigureAwait(false);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"Failed to register service {name} on endpoint {node}");
                }
                return new Entry(this, name, node.Address.ToString(), node.Port, serviceId);
            }

            if (endpoint is DnsEndPoint dns)
            {
                var serviceId = string.Format("{0}-{1}-{2}", name, dns.Host, dns.Port);
                var checkId = string.Format("{0}-{1}", dns.Host, dns.Port);
                
                var acr = new AgentCheckRegistration
                {
                    TCP = dns.ToString(),
                    Name = checkId,
                    ID = checkId,
                    Interval = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                };
                
                var asr = new AgentServiceRegistration
                {
                    Address = dns.Host,
                    Port = dns.Port,
                    ID = serviceId,
                    Name = name,
                    Check = acr
                };

                var res = await _client.Agent.ServiceRegister(asr).ConfigureAwait(false);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"Failed to register service {name} on endpoint {dns}");
                }
                return new Entry(this, name, dns.Host, dns.Port, serviceId);
            }
            
            throw new ArgumentException("The type of endpoint must be one of IPEndPoint or DnsEndPoint", nameof(endpoint));
        }

        public async Task UnregisterServiceAsync(string serviceId)
        {
            await _client.Agent.ServiceDeregister(serviceId).ConfigureAwait(false);
        }
    }

    internal sealed class Entry : IDisposable
    {
        private readonly ServiceDiscovery _serviceDiscovery;

        public Entry(ServiceDiscovery serviceDiscovery, string serviceName, string address, int port,
            string serviceId)
        {
            _serviceDiscovery = serviceDiscovery;
            ServiceName = serviceName;
            ServiceId = serviceId;

            Address = address;
            Port = port;
        }

        public string ServiceName { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string ServiceId { get; set; }
        
        public void Dispose()
        {
            _serviceDiscovery.UnregisterServiceAsync(ServiceId).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}