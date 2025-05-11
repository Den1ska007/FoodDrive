using FoodDrive.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

public class UserConverter : JsonConverter<User>
{
    public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
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
        var typeInfo = new
        {
            value.Role,
            Value = (object)value
        };
        JsonSerializer.Serialize(writer, typeInfo, options);
    }
}