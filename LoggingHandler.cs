using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bede.RefitTest
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly LoggingSettings _loggingSettings;

        public LoggingHandler(LoggingSettings loggingSettings,
            HttpMessageHandler inner)
            : base(inner)
        {
            _loggingSettings = loggingSettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _loggingSettings.LogInformation.Invoke(LogCategories.RequestResponse, $"Client REQUEST: {request.RequestUri}");

            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _loggingSettings.LogError.Invoke(LogCategories.RequestResponse, ex, ex.Message);
                throw;
            }

            _loggingSettings.LogInformation.Invoke(LogCategories.RequestResponse, $"Client Response: {httpResponseMessage.StatusCode}"); 

            return httpResponseMessage;
        }
    }
}