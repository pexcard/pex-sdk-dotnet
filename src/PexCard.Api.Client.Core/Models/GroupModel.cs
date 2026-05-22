namespace PexCard.Api.Client.Core.Models
{
    public class GroupModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Id of the User Group this legacy group corresponds to, when applicable; otherwise null.
        /// </summary>
        public long? UserGroupId { get; set; }
    }
}
