using System;

namespace PexCard.Api.Client.Core.Models
{
    public class UpsertCallbackSubscriptionModel
    {
        public UpsertCallbackSubscriptionModel()
        {
        }

        public UpsertCallbackSubscriptionModel(CallbackType callbackType, CallbackStatus status, Uri url, string name = null, string description = null)
        {
            CallbackType = callbackType;
            Status = status;
            Url = url;
            Name = name;
            Description = description;
        }

        public CallbackType CallbackType { get; set; }

        public CallbackStatus Status { get; set; }

        public Uri Url { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
