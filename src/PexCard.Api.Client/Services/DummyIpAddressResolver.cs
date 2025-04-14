using PexCard.Api.Client.Core.Interfaces;

namespace PexCard.Api.Client
{
    public class DummyIpAddressResolver : IIPAddressResolver
    {
        public string GetValue() => null;
    }
}