namespace PexCard.Api.Client.Core.Models
{
    public class TagTextDataModel : TagDetailsModel
    {
        public int Length { get; set; }

        public TagTextValidationType ValidationType { get; set; }
    }
}
