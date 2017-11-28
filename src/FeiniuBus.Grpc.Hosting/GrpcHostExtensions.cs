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
            var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            token.Register(state =>
            {
                var tcs = (TaskCompletionSource<object>) state;
                tcs.TrySetResult(null);
            }, waitForStop);

            await waitForStop.Task;

            await host.StopAsync(token);
        }
    }
}