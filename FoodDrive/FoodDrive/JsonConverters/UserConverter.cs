// JsonConverters/UserConverter.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using FoodDrive.Models;

// Data/JsonStorage.cs
public class UserConverter : JsonConverter<User>
{
    public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        var role = root.GetProperty("Role").GetString();
        return role switch
        {
            "Admin" => JsonSerializer.Deserialize<Admin>(root.GetRawText(), options),
            "Customer" => JsonSerializer.Deserialize<Customer>(root.GetRawText(), options),
            _ => throw new JsonException("Unknown role")
        };
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}
