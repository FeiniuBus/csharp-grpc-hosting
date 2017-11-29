using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class ConfigureBuilder
    {
        public ConfigureBuilder(MethodInfo configure)
        {
            MethodInfo = configure;
        }
        
        public MethodInfo MethodInfo { get; }

        public Action<IApplicationBuilder> Build(object instance) => builder => Invoke(instance, builder);

        private void Invoke(object instance, IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var parameterInfos = MethodInfo.GetParameters();
                var parameters = new object[parameterInfos.Length];

                for (int index = 0; index < parameterInfos.Length; index++)
                {
                    var parameterInfo = parameterInfos[index];
                    if (parameterInfo.ParameterType == typeof(IApplicationBuilder))
                    {
                        parameters[index] = builder;
                    }
                    else
                    {
                        try
                        {
                            parameters[index] = serviceProvider.GetRequiredService(parameterInfo.ParameterType);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("Could not resolve a service of type '{0}' for the parameter '{1}' of method '{2}' on type '{3}'.",
                                parameterInfo.ParameterType.FullName,
                                parameterInfo.Name,
                                MethodInfo.Name,
                                MethodInfo.DeclaringType.FullName), e);
                        }
                    }
                }

                MethodInfo.Invoke(instance, parameters);
            }
        }
    }
}