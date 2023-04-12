using System;
using System.Net;

namespace PexCard.Api.Client.Core.Exceptions
{
    public class PexAuthClientException : Exception
    {
        public HttpStatusCode Code { get; }

        public string CorrelationId { get; }

        public PexAuthClientException(HttpStatusCode code, string responseContent, string correlationId = default)
            : base(responseContent)
        {
            Code = code;
            CorrelationId = correlationId;
        }

        public PexAuthClientException(HttpStatusCode code, string responseContent, Exception innerException, string correlationId = default)
            : base(responseContent, innerException)
        {
            Code = code;
            CorrelationId = correlationId;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(CorrelationId))
            {
                return $"{(int)Code} {Code}: {base.ToString()}";
            }
            else
            {
                return $"[{CorrelationId}] {(int)Code} {Code}: {base.ToString()}";
            }
        }
    }
}
