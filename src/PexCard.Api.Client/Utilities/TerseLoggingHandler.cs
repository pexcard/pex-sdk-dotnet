using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    // based on https://github.com/serilog/serilog-aspnetcore/blob/main/src/Serilog.AspNetCore/AspNetCore/RequestLoggingMiddleware.cs
    // but using dotnet logger. much more terse and easier to read http request log messages.
    internal class TerseLoggingHandler<TClient> : DelegatingHandler
    {
        private readonly ILogger _logger;
        private readonly LogLevel _successLogLevel;
        private readonly LogLevel _failureLogLevel;

        public TerseLoggingHandler(ILogger<TClient> logger, LogLevel successLogLevel = LogLevel.Information, LogLevel failureLogLevel = LogLevel.Warning)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
            _successLogLevel = successLogLevel;
            _failureLogLevel = failureLogLevel;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            async Task<HttpResponseMessage> SendAsync()
            {
                var start = Stopwatch.GetTimestamp();
                var response = await base.SendAsync(request, cancellationToken);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                if (response.IsSuccessStatusCode)
                {
                    LogCompletion(_successLogLevel, response, elapsedMs);
                }
                else
                {
                    LogCompletion(_failureLogLevel, response, elapsedMs);
                }

                return response;
            }

            return SendAsync();
        }

        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

        private void LogCompletion(LogLevel logLevel, HttpResponseMessage response, double elapsedMs, Exception ex = null)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            _logger.Log(logLevel, ex, "HTTP {RequestMethod} {RequestUri} responded {StatusCode} in {Elapsed:0.0000} ms", response.RequestMessage?.Method, response.RequestMessage?.RequestUri, (int)response.StatusCode, elapsedMs);
        }
    }
}
