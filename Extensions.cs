using System.Text.Json;
using System.Text.Json.Serialization;

public class IntOrEnumConverter : JsonConverter<GatewayOpcodes>
{
    public override GatewayOpcodes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            int value = reader.GetInt32();
            return Enum.IsDefined(typeof(GatewayOpcodes), value) ? (GatewayOpcodes)value : GatewayOpcodes.Unknown;
        }
        else
        {
            throw new JsonException("Expected an integer or a valid enum value.");
        }
    }

    public override void Write(Utf8JsonWriter writer, GatewayOpcodes value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}