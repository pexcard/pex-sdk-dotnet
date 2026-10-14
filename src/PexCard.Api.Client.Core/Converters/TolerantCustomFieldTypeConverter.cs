using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Converters
{
    internal class TolerantCustomFieldTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (Nullable.GetUnderlyingType(objectType) ?? objectType) == typeof(CustomFieldType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return Nullable.GetUnderlyingType(objectType) != null ? null : (object)CustomFieldType.Unknown;
                case JsonToken.Integer:
                    return reader.Value is BigInteger ? CustomFieldType.Unknown : FromNumber(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
                case JsonToken.String:
                    return FromString((string)reader.Value);
                default:
                    reader.Skip();
                    return CustomFieldType.Unknown;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // An attribute converter outranks the caller's settings converters, so hand off to theirs (e.g. StringEnumConverter) to keep their output.
            var settingsConverter = serializer.Converters.FirstOrDefault(c => !(c is TolerantCustomFieldTypeConverter) && c.CanWrite && c.CanConvert(value.GetType()));
            if (settingsConverter != null)
            {
                settingsConverter.WriteJson(writer, value, serializer);
                return;
            }

            writer.WriteValue((int)(CustomFieldType)value);
        }

        private static CustomFieldType FromString(string value)
        {
            var text = value?.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return CustomFieldType.Unknown;
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

            return CustomFieldType.Unknown;
        }

        private static CustomFieldType FromNumber(long number)
        {
            if (number < int.MinValue || number > int.MaxValue)
            {
                return CustomFieldType.Unknown;
            }

            return (CustomFieldType)(int)number;
        }
    }
}
