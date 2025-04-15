// JsonConverters/UserConverter.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using FoodDrive.Models;

public class UserConverter : JsonConverter<User>
{
    public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            if (root.TryGetProperty("Role", out JsonElement roleElement))
            {
                string role = roleElement.GetString();
                return role switch
                {
                    "Admin" => JsonSerializer.Deserialize<Admin>(root.GetRawText(), options),
                    "Customer" => JsonSerializer.Deserialize<Customer>(root.GetRawText(), options),
                    _ => throw new JsonException($"Unknown role: {role}")
                };
            }
            throw new JsonException("Role property not found");
        }
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}