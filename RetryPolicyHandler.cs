using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Bede.RefitTest
{
    public class RetryPolicyHandler : DelegatingHandler
    {
        private readonly bool _retryPolicyEnabled;

        private static readonly List<HttpStatusCode> TransientStatusCodes = new List<HttpStatusCode>
        {
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.ServiceUnavailable,
        };

        private readonly Policy _retryPolicy;

        public RetryPolicyHandler(bool retryPolicyEnabled, IEnumerable<TimeSpan> retryPolicyIntervals, HttpMessageHandler inner) 
            : base(inner)
        {
            _retryPolicyEnabled = retryPolicyEnabled;
            _retryPolicy = Policy
            .Handle<TransientHttpRequestException>()
            .WaitAndRetryAsync(retryPolicyIntervals);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            Console.WriteLine($"Retry handler: {request.RequestUri}");

            HttpResponseMessage responseMessage;
            if (_retryPolicyEnabled)
            {
                responseMessage = await _retryPolicy.ExecuteAsync(async () =>
                {
                    Console.WriteLine($"Retry handler calling: {request.RequestUri}");

                    var result = await base.SendAsync(request, cancellationToken);

                    if (TransientStatusCodes.Contains(result.StatusCode))
                        throw new TransientHttpRequestException("Transient http request exception", result.StatusCode);

                    return result;
                });
            }
            else
            {
                responseMessage = await base.SendAsync(request, cancellationToken);
            }

            Console.WriteLine($"Retry handler response: {responseMessage.StatusCode}");

            return responseMessage;
        }
    }
}