using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Converters
{
    internal class UnknownEnumNameAsNullConverter : StringEnumConverter
    {
        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException) when (Nullable.GetUnderlyingType(objectType) != null)
            {
                return null;
            }
        }
    }
}
