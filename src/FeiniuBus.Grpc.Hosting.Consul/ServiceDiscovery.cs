using System;
using System.Net;
using System.Threading;
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

        public async Task<string> RegisterServiceAsync(string name, EndPoint endpoint, CancellationToken token = default (CancellationToken))
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

                var res = await _client.Agent.ServiceRegister(asr, token).ConfigureAwait(false);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"Failed to register service {name} on endpoint {node}");
                }
                return serviceId;
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

                var res = await _client.Agent.ServiceRegister(asr, token).ConfigureAwait(false);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"Failed to register service {name} on endpoint {dns}");
                }
                return serviceId;
            }
            
            throw new ArgumentException("The type of endpoint must be one of IPEndPoint or DnsEndPoint", nameof(endpoint));
        }

        public async Task UnregisterServiceAsync(string serviceId, CancellationToken token = default (CancellationToken))
        {
            await _client.Agent.ServiceDeregister(serviceId, token).ConfigureAwait(false);
        }
    }
}