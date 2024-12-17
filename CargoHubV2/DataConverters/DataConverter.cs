using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Globalization;

namespace CargohubV2.DataConverters
{
    public class FlexibleDateTimeConverter : DateTimeConverterBase
    {
        private readonly string[] _formats = new[]
        {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ssZ"
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(_formats[0]));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.Date)
                return reader.Value;

            if (DateTime.TryParseExact((string)reader.Value, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
            {
                return date;
            }

            throw new JsonSerializationException("Invalid date format");
        }
    }
}
