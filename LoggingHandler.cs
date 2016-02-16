using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bede.RefitTest
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly bool _loggingEnabled;

        public LoggingHandler(bool loggingEnabled, HttpMessageHandler inner)
            : base(inner)
        {
            _loggingEnabled = loggingEnabled;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if(_loggingEnabled)
                Console.WriteLine($"Request: {request.RequestUri}");

            var foo = await base.SendAsync(request, cancellationToken);

            if (_loggingEnabled)
                Console.WriteLine($"Response: {foo.StatusCode}");

            return foo;
        }
    }
}