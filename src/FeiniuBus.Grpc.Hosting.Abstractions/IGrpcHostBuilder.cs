using System;
using Microsoft.Extensions.Configuration;
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
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        IGrpcHostBuilder ConfigureAppConfiguration(
            Action<GrpcHostBuilderContext, IConfigurationBuilder> configureDelegate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceTypes"></param>
        /// <returns></returns>
        IGrpcHostBuilder BindServices(params Type[] serviceTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        IGrpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        IGrpcHostBuilder ConfigureServices(Action<GrpcHostBuilderContext, IServiceCollection> configureServices);

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