﻿namespace PexCard.Api.Client.Core.Models
{
    public class TagTextDataModel : TagDataModel
    {
        public int MinLength { get; set; }

        public int Length { get; set; }

        public TagTextValidationType ValidationType { get; set; }
    }
}
