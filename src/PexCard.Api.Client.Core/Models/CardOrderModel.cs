using System;
using System.Collections.Generic;
using System.Text;

namespace PexCard.Api.Client.Core.Models
{
    public class CardOrderModel
    {
        /// <summary> Cards to create </summary>
        public List<CardOrderLineModel> Cards { get; set; }

        /// <summary>
        /// Normalize NormalizeHomeAddress Address
        /// </summary>
        public bool NormalizeHomeAddress { get; set; }
        /// <summary>
        /// Normalize Shipping Address
        /// </summary>
        public bool NormalizeShippingAddress { get; set; }
    }
}
