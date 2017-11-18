using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeiniuBus.Grpc.Hosting.Consul
{
    public static class ConsulGrpcHostExtensions
    {
        public static void Run(this ConsulGrpcHost host, string name, string address, int port)
        {
            host.RunAsync(name, address, port).GetAwaiter().GetResult();
        }
        
        private static async Task RunAsync(this ConsulGrpcHost host, string name, string address, int port,
            CancellationToken token = default(CancellationToken))
        {
            if (token.CanBeCanceled)
            {
                await host.RunAsync(name, address, port, token, null);
                return;
            }
            
            var done = new ManualResetEventSlim(false);
            using (var cts = new CancellationTokenSource())
            {
                AttachCtrlcSigtermShutdown(cts, done, "Application is shutting down...");
                await host.RunAsync(name, address, port, cts.Token, "Application started. Press Ctrl+C to shut down.");
                done.Set();
            }
        }

        private static async Task RunAsync(this ConsulGrpcHost host, string name, string address, int port,
            CancellationToken token, string shutdownMessage)
        {
            await host.StartAsync(name, address, port, token).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(shutdownMessage))
            {
                Console.WriteLine(shutdownMessage);
            }

            await host.WaitForTokenShutdownAsync(token);
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

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Shutdown();
            Console.CancelKeyPress += (s, e) =>
            {
                Shutdown();
                e.Cancel = true;
            };
        }

        private static async Task WaitForTokenShutdownAsync(this ConsulGrpcHost host, CancellationToken token)
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