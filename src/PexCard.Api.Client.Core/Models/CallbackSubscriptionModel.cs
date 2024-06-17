using System;

namespace PexCard.Api.Client.Core.Models
{
    public class CallbackSubscriptionModel
    {
        public CallbackSubscriptionModel()
        {
        }

        public CallbackSubscriptionModel(int id, int businessAccountId, CallbackType callbackType, CallbackStatus status, Uri url, string username, DateTime createdDate, string name = null, string description = null)
        {
            Id = id;
            BusinessAccountId = businessAccountId;
            CallbackType = callbackType;
            Status = status;
            Url = url;
            Username = username;
            CreatedDate = createdDate;
            Name = name;
            Description = description;
        }

        public int Id { get; set; }

        public int BusinessAccountId { get; set; }

        public CallbackType CallbackType { get; set; }

        public CallbackStatus Status { get; set; }

        public Uri Url { get; set; }

        public string Username { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
