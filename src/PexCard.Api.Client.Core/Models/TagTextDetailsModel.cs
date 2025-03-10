namespace PexCard.Api.Client.Core.Models
{
    public class TagTextDetailsModel : TagDetailsModel
    {
        public int MinLength { get; set; }

        public int Length { get; set; }

        public TagTextValidationType ValidationType { get; set; }

        public string ValidationRegex { get; set; }
    }
}
