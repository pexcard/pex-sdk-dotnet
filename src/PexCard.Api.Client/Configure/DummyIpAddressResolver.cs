using PexCard.Api.Client.Core.Interfaces;

namespace PexCard.Api.Client.Configure
{
    public class DummyIpAddressResolver : IIPAddressResolver
    {
        public string GetValue() => null;
    }
}