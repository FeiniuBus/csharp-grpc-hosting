using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FeiniuBus.Grpc.Hosting
{
    public static class GrpcHostExtensions
    {
        public static void Run(this IGrpcHost host)
        {
            host.RunAsync().GetAwaiter().GetResult();
        }
        
        private static async Task RunAsync(this IGrpcHost host, CancellationToken token = default(CancellationToken))
        {
            if (token.CanBeCanceled)
            {
                await host.RunAsync(token, null);
                return;
            }
            
            var done = new ManualResetEventSlim(false);
            using (var cts = new CancellationTokenSource())
            {
                AttachCtrlcSigtermShutdown(cts, done, "Application is shutting down...");

                await host.RunAsync(cts.Token, "Application started. Press Ctrl+C to shut down.");
                done.Set();
            }
        }
        
        private static async Task RunAsync(this IGrpcHost host, CancellationToken token, string shutdownMessage)
        {
            using (host)
            {
                host.Start();
                
                var hostingEnvironment = host.Services.GetService<IHostingEnvironment>();
                
                Console.WriteLine($"Hosting environment: {hostingEnvironment.EnvironmentName}");
                Console.WriteLine($"Content root path: {hostingEnvironment.ContentRootPath}");

                foreach (var port in host.Server.Ports)
                {
                    Console.WriteLine($"Now listening on: {port.Host}:{port.Port}");
                }

                if (!string.IsNullOrEmpty(shutdownMessage))
                {
                    Console.WriteLine(shutdownMessage);
                }

                await host.WaitForTokenShutdownAsync(token);
            }
        }

        private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim resetEvent,
            string shutdownMessage)
        {
            void Shutdown()
            {
                if (!cts.IsCancellationRequested)
                {
                    if (!string.IsNullOrEmpty(shutdownMessage))
                    {
                        Console.WriteLine(shutdownMessage);
                    }
                    try
                    {
                        cts.Cancel();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }

                resetEvent.Wait();
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Shutdown();

                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
        }

        private static async Task WaitForTokenShutdownAsync(this IGrpcHost host, CancellationToken token)
        {
            var applicationLifetime = host.Services.GetService<IApplicationLifetime>();
            token.Register(state =>
            {
                ((IApplicationLifetime)state).StopApplication();
            }, applicationLifetime);
            
            var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            applicationLifetime.ApplicationStopping.Register(obj =>
            {
                var tcs = (TaskCompletionSource<object>)obj;
                tcs.TrySetResult(null);
            }, waitForStop);

            await waitForStop.Task;

            // ReSharper disable once MethodSupportsCancellation
            await host.StopAsync();
        }
    }
}