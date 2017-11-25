using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.Grpc.Hosting.Internal
{
    public class ApplicationLifetime : IApplicationLifetime
    {
        private readonly CancellationTokenSource _startedSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stoppingSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stoppedSource = new CancellationTokenSource();
        private readonly ILogger<ApplicationLifetime> _logger;

        public ApplicationLifetime(ILogger<ApplicationLifetime> logger)
        {
            _logger = logger;
        }

        public CancellationToken ApplicationStarted => _startedSource.Token;

        public CancellationToken ApplicationStopping => _stoppingSource.Token;

        public CancellationToken ApplicationStopped => _stoppedSource.Token;
        
        public void StopApplication()
        {
            lock (_stoppingSource)
            {
                try
                {
                    ExecuteHandlers(_stoppingSource);
                }
                catch (Exception e)
                {
                    _logger.ApplicationError(LoggerEventIds.ApplicationStoppingException,
                        "An error occurred stopping the application", e);
                }
            }
        }

        public void NotifyStarted()
        {
            try
            {
                ExecuteHandlers(_startedSource);
            }
            catch (Exception e)
            {
                _logger.ApplicationError(LoggerEventIds.ApplicationStartupException,
                    "An error occurred starting the application", e);
            }
        }

        public void NotifyStopped()
        {
            try
            {
                ExecuteHandlers(_stoppedSource);
            }
            catch (Exception e)
            {
                _logger.ApplicationError(LoggerEventIds.ApplicationStoppedException,
                    "An error occurred stopping the application", e);
            }
        }

        private void ExecuteHandlers(CancellationTokenSource cancel)
        {
            if (cancel.IsCancellationRequested)
            {
                return;
            }

            List<Exception> exceptions = null;
            try
            {
                cancel.Cancel(false);
            }
            catch (Exception e)
            {
                exceptions = new List<Exception> {e};
            }

            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}