using System;
using System.Net;
using System.Net.Http;

namespace Bede.RefitTest
{
    public class TransientHttpRequestException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; set; }

        public TransientHttpRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public TransientHttpRequestException(string message, HttpStatusCode statusCode, Exception inner)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}