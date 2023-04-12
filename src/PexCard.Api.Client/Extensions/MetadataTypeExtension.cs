using System;
using System.Linq;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Extensions
{
    internal static class MetadataTypeExtension
    {
        public static bool IsReceipt(this MetadataType _this)
        {
            var receiptMetadataTypes = new[] { MetadataType.Image, MetadataType.Pdf };
            return receiptMetadataTypes.Contains(_this);
        }
    }
}
