using System;
using System.Net;

namespace PexCard.Api.Client.Core.Exceptions
{
    /// <summary>
    /// Thrown when a call to the PEX OAuth Server (discovery / token / refresh) fails. The message
    /// carries the raw response body, which for OAuth errors is the RFC 6749 <c>{ error,
    /// error_description }</c> payload.
    /// </summary>
    public class PexOAuthClientException : Exception
    {
        public HttpStatusCode Code { get; }

        public string CorrelationId { get; }

        public PexOAuthClientException(HttpStatusCode code, string responseContent, string correlationId = default)
            : base(responseContent)
        {
            Code = code;
            CorrelationId = correlationId;
        }

        public PexOAuthClientException(HttpStatusCode code, string responseContent, Exception innerException, string correlationId = default)
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
