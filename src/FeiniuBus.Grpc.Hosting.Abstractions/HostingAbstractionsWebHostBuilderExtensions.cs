using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace FeiniuBus.Grpc.Hosting
{
    public static class HostingAbstractionsWebHostBuilderExtensions
    {
        private static readonly string ServerUrlsSeparator = ",";

        public static IGrpcHostBuilder UseConfiguration(this IGrpcHostBuilder builder, IConfiguration configuration)
        {
            foreach (var setting in configuration.AsEnumerable())
            {
                builder.UseSetting(setting.Key, setting.Value);
            }

            return builder;
        }

        public static IGrpcHostBuilder UseContentRoot(this IGrpcHostBuilder builder, string contentRoot)
        {
            if (contentRoot == null)
            {
                throw new ArgumentNullException(nameof(contentRoot));
            }

            return builder.UseSetting(GrpcHostDefaults.ContentRootKey, contentRoot);
        }
        
        public static IGrpcHostBuilder UseUrls(this IGrpcHostBuilder builder, params string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            return builder.UseSetting(GrpcHostDefaults.ServerUrlsKey, string.Join(ServerUrlsSeparator, urls));
        }

        public static IGrpcHostBuilder BindService<TService>(this IGrpcHostBuilder builder) where TService : class
        {
            return builder.BindServices(typeof(TService));
        }
    }
}