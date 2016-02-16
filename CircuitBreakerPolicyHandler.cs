using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Bede.RefitTest
{
    public class CircuitBreakerPolicyHandler : DelegatingHandler
    {
        private readonly bool _circuitBreakerEnabled;

        private readonly Policy _circuitBreaker;

        public CircuitBreakerPolicyHandler(bool circuitBreakerEnabled, int exceptionAllowedBeforeCircuitBroken, int durationOfCircuitBreakMiliseconds, HttpMessageHandler inner) 
            : base(inner)
        {
            _circuitBreakerEnabled = circuitBreakerEnabled;
            

            _circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(exceptionAllowedBeforeCircuitBroken, TimeSpan.FromMilliseconds(durationOfCircuitBreakMiliseconds));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Circuit breaker handler: {request.RequestUri}");

            HttpResponseMessage responseMessage;
            if (_circuitBreakerEnabled)
            {
                responseMessage =
                    await _circuitBreaker.ExecuteAsync(async () => await base.SendAsync(request, cancellationToken));
            }
            else
            {
                responseMessage = await base.SendAsync(request, cancellationToken);
            }

            Console.WriteLine($"Circuit breaker response: {responseMessage.StatusCode}");

            return responseMessage;
        }
    }
}