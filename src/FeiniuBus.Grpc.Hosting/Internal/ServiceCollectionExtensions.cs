using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection Clone(this IServiceCollection services)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in services)
            {
                clone.Add(service);
            }

            return clone;
        }
    }
}