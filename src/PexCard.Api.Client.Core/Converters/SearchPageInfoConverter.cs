using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core.Converters
{
    internal class SearchPageInfoConverter : JsonConverter<PageInfoModel>
    {
        public override bool CanWrite => false;

        public override PageInfoModel ReadJson(JsonReader reader, Type objectType, PageInfoModel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var pageInfo = JObject.Load(reader);

            return new PageInfoModel
            {
                Page = ReadFirstValue<int>(pageInfo, "PageNumber", nameof(PageInfoModel.Page)),
                PageSize = ReadFirstValue<int>(pageInfo, nameof(PageInfoModel.PageSize)),
                TotalItems = ReadFirstValue<long>(pageInfo, "TotalCount", nameof(PageInfoModel.TotalItems)),
            };
        }

        public override void WriteJson(JsonWriter writer, PageInfoModel value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        private static T ReadFirstValue<T>(JObject source, params string[] propertyNames) where T : struct
        {
            foreach (var propertyName in propertyNames)
            {
                var token = source.GetValue(propertyName, StringComparison.OrdinalIgnoreCase);
                if (token != null && token.Type != JTokenType.Null)
                {
                    return token.ToObject<T>();
                }
            }

            return default;
        }
    }
}
