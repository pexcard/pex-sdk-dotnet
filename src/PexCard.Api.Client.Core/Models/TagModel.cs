using PexCard.Api.Client.Core.Enums;
using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagModel : TagDataModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Features the tag is shown in (<see cref="TagFeatureSource.Transaction"/>, <see cref="TagFeatureSource.Reimbursement"/>, <see cref="TagFeatureSource.BillPay"/>).
        /// An unrestricted tag lists every feature. Null when the API did not return the field.
        /// A feature this SDK does not know yet is read as <see cref="TagFeatureSource.Unknown"/>.
        /// </summary>
        public List<TagFeatureSource> FeatureSources { get; set; }
    }
}
