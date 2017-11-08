using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal class StartupLoader
    {
        public static StartupMethods LoadMethods(IServiceProvider hostingServiceProvider, Type startupType,
            string environmentName)
        {
            throw new NotImplementedException();
        }
        
        private static ConfigureContainerBuilder FindConfigureContainerDelegate(Type startupType, string environmentName)
        {
            var configureMethod = FindMethod(startupType, "Configure{0}Container", environmentName, typeof(void), required: false);
            return new ConfigureContainerBuilder(configureMethod);
        }

        private static ConfigureServicesBuilder FindConfigureServicesDelegate(Type startupType, string environmentName)
        {
            var servicesMethod = FindMethod(startupType, "Configure{0}Services", environmentName, typeof(IServiceProvider), required: false)
                                 ?? FindMethod(startupType, "Configure{0}Services", environmentName, typeof(void), required: false);
            return new ConfigureServicesBuilder(servicesMethod);
        }

        private static MethodInfo FindMethod(Type startupType, string methodName, string environmentName,
            Type returnType = null, bool required = true)
        {
            var methodNameWithEnv = string.Format(CultureInfo.InvariantCulture, methodName, environmentName);
            var methodNameWithNoEnv = string.Format(CultureInfo.InvariantCulture, methodName, "");

            var methods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithEnv)).ToList();
            if (selectedMethods.Count > 1)
            {
                throw new InvalidOperationException(string.Format("Having multiple overloads of method '{0}' is not supported.", methodNameWithEnv));
            }

            if (selectedMethods.Count == 0)
            {
                selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithNoEnv)).ToList();
                if (selectedMethods.Count > 1)
                {
                    throw new InvalidOperationException(string.Format("Having multiple overloads of method '{0}' is not supported.", methodNameWithNoEnv));
                }
            }

            var methodInfo = selectedMethods.FirstOrDefault();
            if (methodInfo == null)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("A public method named '{0}' or '{1}' could not be found in the '{2}' type.",
                        methodNameWithEnv,
                        methodNameWithNoEnv,
                        startupType.FullName));
                }
                return null;
            }

            if (returnType != null && methodInfo.ReturnType != returnType)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("The '{0}' method in the type '{1}' must have a return type of '{2}'.",
                        methodInfo.Name,
                        startupType.FullName,
                        returnType.Name));
                }
                return null;
            }

            return methodInfo;
        }
    }
}