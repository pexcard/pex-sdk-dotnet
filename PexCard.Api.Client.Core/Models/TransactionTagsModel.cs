using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionTagsModel
    {
        public List<TransactionTagModel> Tags { get; set; }
        public FieldsState State { get; set; }
        public string FieldsVersion { get; set; }
    }
}