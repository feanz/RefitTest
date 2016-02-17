using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Bede.RefitTest
{
    public class CircuitBreakerPolicyHandler : DelegatingHandler
    {
        private readonly LoggingSettings _loggingSettings;
        private readonly bool _circuitBreakerEnabled;

        private readonly Policy _circuitBreaker;

        public CircuitBreakerPolicyHandler(LoggingSettings loggingSettings, CircuitBreakerSettings settings, HttpMessageHandler inner)
            : base(inner)
        {
            _loggingSettings = loggingSettings;
            _circuitBreakerEnabled = settings.CircuitBreakerEnabled;

            _circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(settings.ExceptionAllowedBeforeCircuitBroken, TimeSpan.FromMilliseconds(settings.DurationOfCircuitBreakMiliseconds));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage;
            if (_circuitBreakerEnabled)
            {
                responseMessage = await _circuitBreaker.ExecuteAsync(async () =>
                    {
                        _loggingSettings.LogInformation(LogCategories.CircuitBreaker, "Circuit breaker handler calling " + request.RequestUri);

                        try
                        {
                            return await base.SendAsync(request, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _loggingSettings.LogError(LogCategories.CircuitBreaker, ex,
                                "Circuit breaker caught exception");
                            throw;
                        }
                    });
            }
            else
            {
                responseMessage = await base.SendAsync(request, cancellationToken);
            }

            return responseMessage;
        }
    }
}