

using System;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    internal static class HostingLoggerExtensions
    {
        public static void Starting(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    LoggerEventIds.Starting,
                    "Hosting starting");
            }
        }
        
        public static void Started(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    LoggerEventIds.Started,
                    "Hosting started");
            }
        }

        public static void Shutdown(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    LoggerEventIds.Shutdown,
                    "Hosting shutdown");
            }
        }

        public static void ServerShutdownException(this ILogger logger, Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    LoggerEventIds.ServerShutdownException,
                    ex,
                    "Server shutdown exception");
            }
        }
    }
}