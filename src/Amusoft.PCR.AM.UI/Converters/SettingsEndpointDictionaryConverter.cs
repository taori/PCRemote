using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Amusoft.PCR.AM.UI.Converters;

public class SettingsEndpointDictionaryConverter : JsonConverter<Dictionary<IPEndPoint, Guid>>
{
	public override Dictionary<IPEndPoint, Guid>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read())
			return default;

		var values = new Dictionary<IPEndPoint, Guid>();

		do
		{
			if (reader.TokenType == JsonTokenType.StartObject)
			{
				reader.Read();
				if (reader.TokenType != JsonTokenType.PropertyName)
					throw new Exception("Parsing error");
				// k
				reader.Read();
				var key = reader.GetString();

				reader.Read();
				// v
				reader.Read();
				var value = reader.GetString();
				reader.Read();
				// endobject
				reader.Read();
				// startobject/endarray

				if (key is not null && value is not null)
					values[IPEndPoint.Parse(key)] = Guid.Parse(value);
			}
		} while (reader.TokenType != JsonTokenType.EndArray);

		return values;
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<IPEndPoint, Guid> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var pair in value)
		{
			writer.WriteStartObject();
			writer.WriteString("k", pair.Key.ToString());
			writer.WriteString("v", pair.Value);
			writer.WriteEndObject();
		}

		writer.WriteEndArray();
	}
}