using System;
using System.Reflection;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    public static class RpcServcieLoader
    {
        public static ServerServiceDefinition LoadService(IServiceProvider applicationServices, Type serviceType)
        {
            var method = FindBindServiceMethod(serviceType);
            var instance = ActivatorUtilities.GetServiceOrCreateInstance(applicationServices, serviceType);
            return method.Invoke(null, new[] {instance}) as ServerServiceDefinition;
        }

        private static MethodInfo FindBindServiceMethod(Type serviceType)
        {
            var baseType = FindBaseType(serviceType);
            var findType = baseType;
            if (baseType.IsNested)
            {
                findType = baseType.DeclaringType;
            }

            var method = findType.GetMethod("BindService", BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                throw new NotSupportedException("gRPC framework has changed already");
            }

            return method;
        }

        private static Type FindBaseType(Type serviceType)
        {
            if (serviceType.BaseType == typeof(object))
            {
                return serviceType;
            }
            
            var type = serviceType.BaseType;
            while (type != typeof(object))
            {
                type = type.BaseType;
            }

            return type;
        }
    }
}