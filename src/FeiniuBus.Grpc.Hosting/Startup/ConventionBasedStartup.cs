using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using FeiniuBus.Grpc.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Startup
{
    internal class ConventionBasedStartup : IStartup
    {
        private readonly StartupMethods _methods;

        public ConventionBasedStartup(StartupMethods methods)
        {
            _methods = methods;
        }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                return _methods.ConfigureServicesDelegate(services);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }

                throw;
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            try
            {
                _methods.ConfigureDelegate(app);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }

                throw;
            }
        }
    }
}