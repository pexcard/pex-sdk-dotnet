using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Accounting app / ingestion channel a bill inbox item came from.
    /// <see cref="Email"/> is reserved for the internal PEX inbound-email channel and is rejected
    /// on POST /BillInbox; it only appears on responses.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BillInboxSource
    {
        /// <summary>
        /// Forward-compat fallback for unknown values returned by the server.
        /// Not a valid value to send.
        /// </summary>
        Unknown = 0,
        Email = 1,
        QuickBooksOnline = 2,
        SageIntacct = 3,
        Xero = 4,
        Blackbaud = 5,
        Aplos = 6,
        MicrosoftDynamics365 = 7,
        NetSuite = 8
    }
}
