using Newtonsoft.Json;
using PexCard.Api.Client.Core.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Feature a tag is shown in. A tag definition lists every feature it is shown in
    /// (<c>FeatureSources</c>); an unrestricted tag lists all of them.
    /// Names the SDK does not know deserialize as <see cref="Unknown"/> instead of throwing;
    /// numbers the SDK does not know keep their value.
    /// </summary>
    [JsonConverter(typeof(TolerantTagFeatureSourceConverter))]
    public enum TagFeatureSource
    {
        Unknown = 0,
        /// <summary>Card transactions</summary>
        Transaction = 1,
        /// <summary>Reimbursements</summary>
        Reimbursement = 2,
        /// <summary>Bill payments</summary>
        BillPay = 3
    }
}
