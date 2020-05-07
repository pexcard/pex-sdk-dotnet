using System;
using System.Net;

namespace PexCard.Api.Client.Core.Exceptions
{
    public class PexApiClientException : Exception
    {
        public HttpStatusCode Code { get; }

        public PexApiClientException(HttpStatusCode code, string responseContent)
            : base(responseContent)
        {
            Code = code;
        }

        public override string ToString()
        {
            var result = $"{(int) Code} {Code}: {base.ToString()}";
            return result;
        }
    }
}
