﻿namespace PexCard.Api.Client.Core.Models
{
    public class TagValueModel
    {
        public object Value { get; set; }

        public bool IsAutoGenerated { get; set; }

        public TagModel Definition { get; set; }
    }
}
