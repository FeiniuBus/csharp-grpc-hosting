using System;
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
        
        public static IGrpcHostBuilder UseUrls(this IGrpcHostBuilder builder, params string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            return builder.UseSetting(GrpcHostDefaults.ServerUrlsKey, string.Join(ServerUrlsSeparator, urls));
        }
    }
}