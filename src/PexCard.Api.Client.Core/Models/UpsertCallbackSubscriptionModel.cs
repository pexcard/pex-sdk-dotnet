using System;

namespace PexCard.Api.Client.Core.Models
{
    public class UpsertCallbackSubscriptionModel
    {
        public UpsertCallbackSubscriptionModel()
        {
        }

        public UpsertCallbackSubscriptionModel(CallbackType callbackType, CallbackStatus status, Uri url)
        {
            CallbackType = callbackType;
            Status = status;
            Url = url;
        }

        public CallbackType CallbackType { get; set; }

        public CallbackStatus Status { get; set; }

        public Uri Url { get; set; }
    }
}
