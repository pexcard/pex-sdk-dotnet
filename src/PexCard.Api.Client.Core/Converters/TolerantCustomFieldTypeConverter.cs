using System;
using System.Globalization;
using System.Numerics;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Converters
{
    internal class TolerantCustomFieldTypeConverter : JsonConverter
    {
        private const CustomFieldType Unknown = (CustomFieldType)(-1);

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return (Nullable.GetUnderlyingType(objectType) ?? objectType) == typeof(CustomFieldType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return Nullable.GetUnderlyingType(objectType) != null ? null : (object)Unknown;
                case JsonToken.Integer:
                    return reader.Value is BigInteger ? Unknown : FromNumber(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
                case JsonToken.String:
                    return FromString((string)reader.Value);
                default:
                    reader.Skip();
                    return Unknown;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        private static CustomFieldType FromString(string value)
        {
            var text = value?.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return Unknown;
            }

            if (long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number))
            {
                return FromNumber(number);
            }

            foreach (var name in Enum.GetNames(typeof(CustomFieldType)))
            {
                if (string.Equals(name, text, StringComparison.OrdinalIgnoreCase))
                {
                    return (CustomFieldType)Enum.Parse(typeof(CustomFieldType), name);
                }
            }

            return Unknown;
        }

        private static CustomFieldType FromNumber(long number)
        {
            if (number < int.MinValue || number > int.MaxValue)
            {
                return Unknown;
            }

            var type = (CustomFieldType)(int)number;
            return Enum.IsDefined(typeof(CustomFieldType), type) ? type : Unknown;
        }
    }
}
