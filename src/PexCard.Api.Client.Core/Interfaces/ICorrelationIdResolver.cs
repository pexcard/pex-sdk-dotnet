namespace PexCard.Api.Client.Core.Interfaces
{
    public interface ICorrelationIdResolver
    {
        string CorrelationId { get; }

        string GetValue();
    }
}
