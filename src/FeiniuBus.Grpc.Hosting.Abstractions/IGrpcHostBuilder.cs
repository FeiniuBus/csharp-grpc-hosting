using System;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public interface IGrpcHostBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGrpcHost Build();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        IGrpcHostBuilder BindServices(Type serviceType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        IGrpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetSetting(string key);

        /// <summary>
        /// Add or replace a setting in the configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IGrpcHostBuilder UseSetting(string key, string value);
    }
}