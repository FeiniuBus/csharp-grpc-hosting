using System;
using System.Collections.Generic;
using System.IO;
using FeiniuBus.Grpc.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public class GrpcHostBuilder : IGrpcHostBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;
        private readonly List<Type> _serviceTypes;
        private readonly IConfiguration _config;
        private bool _grpcHostBuilt;

        public GrpcHostBuilder()
        {
            _hostingEnvironment = new HostingEnvironment();
            _config = new ConfigurationBuilder().AddEnvironmentVariables("GRPCHOST_").Build();
            _configureServicesDelegates = new List<Action<IServiceCollection>>();
            _serviceTypes = new List<Type>();

            if (string.IsNullOrEmpty(GetSetting(GrpcHostDefaults.ServerUrlsKey)))
            {
                UseSetting(GrpcHostDefaults.ServerUrlsKey,
                    Environment.GetEnvironmentVariable("GRPCHOST_SERVER.URLS"));
            }
        }
        
        public IGrpcHost Build()
        {
            if (_grpcHostBuilt)
            {
                throw new InvalidOperationException("GrpcHostBuilder allows creation only of a single instance of GrpcHost");
            }
            _grpcHostBuilt = true;

            var hostingServices = BuildCommonServices();
            var applicationServices = hostingServices.Clone();
            var hostingServiceProvider = hostingServices.BuildServiceProvider();

            var host = new GrpcHost(applicationServices, hostingServiceProvider, _config, _serviceTypes);
            host.Initialize();
            return host;
        }

        public IGrpcHostBuilder BindServices(params Type[] serviceTypes)
        {
            if (serviceTypes == null || serviceTypes.Length == 0)
            {
                throw new ArgumentNullException(nameof(serviceTypes));
            }

            _serviceTypes.AddRange(serviceTypes);
            return this;
        }

        public IGrpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }
            
            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        public IGrpcHostBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        private string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                return basePath;
            }
            if (Path.IsPathRooted(contentRootPath))
            {
                return contentRootPath;
            }

            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }

        private IServiceCollection BuildCommonServices()
        {
            var contentRootPath =
                ResolveContentRootPath(_config[GrpcHostDefaults.ContentRootKey], AppContext.BaseDirectory);
            _hostingEnvironment.ContentRootPath = contentRootPath;

            var environment = _config[GrpcHostDefaults.EnvironmentKey];
            if (!string.IsNullOrEmpty(environment))
            {
                _hostingEnvironment.EnvironmentName = environment;
            }
            
            var services = new ServiceCollection();
            services.AddSingleton(_hostingEnvironment);


            var builder = new ConfigurationBuilder().AddInMemoryCollection(_config.AsEnumerable());
            var configuration = builder.Build();
            services.AddSingleton(configuration);

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();
            services.AddLogging();

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }
            
            return services;
        }
    }
}