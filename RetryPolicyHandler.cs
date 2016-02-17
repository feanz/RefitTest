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
        private readonly LoggingSettings _loggingSettings;
        private readonly RetryPolicySettings _settings;

        private static readonly List<HttpStatusCode> TransientStatusCodes = new List<HttpStatusCode>
        {
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.ServiceUnavailable,
        };

        private readonly Policy _retryPolicy;

        public RetryPolicyHandler(LoggingSettings loggingSettings, RetryPolicySettings settings, HttpMessageHandler inner) 
            : base(inner)
        {
            _loggingSettings = loggingSettings;
            _settings = settings;
            _retryPolicy = Policy
            .Handle<TransientHttpRequestException>()
            .WaitAndRetryAsync(settings.RetryPolicyIntervals);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage;
            if (_settings.RetryPolicyEnabled)
            {
                responseMessage = await _retryPolicy.ExecuteAsync(async () =>
                {
                    _loggingSettings.LogInformation(LogCategories.RetryPolicy, $"Retry handler calling: {request.RequestUri}");

                    var response = await base.SendAsync(request, cancellationToken);

                    if (TransientStatusCodes.Contains(response.StatusCode))
                    {
                        var exception = new TransientHttpRequestException(
                            $"Transient http request exception from calling {request.RequestUri} recieved a response status code {response.StatusCode}", response.StatusCode);
                        _loggingSettings.LogError(LogCategories.RetryPolicy, exception, exception.Message);
                        throw exception;
                    }

                    return response;
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