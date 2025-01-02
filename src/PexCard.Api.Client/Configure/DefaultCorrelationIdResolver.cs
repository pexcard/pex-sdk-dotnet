using System;
using PexCard.Api.Client.Core.Interfaces;

namespace PexCard.Api.Client.Configure
{
    public class DefaultCorrelationIdResolver : ICorrelationIdResolver
    {
        public string CorrelationId => $"ext-{Guid.NewGuid()}";

        public string GetValue()
        {
            return CorrelationId;
        }
    }
}
