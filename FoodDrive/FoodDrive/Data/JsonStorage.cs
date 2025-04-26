using System.Text.Json;
using FoodDrive.Interfaces;
using System.IO;
using System;
using FoodDrive.Models;
using System.Text.Json.Serialization;

namespace FoodDrive.Data
{
    public class JsonStorage<T> : IDataStorage<T> where T : class, IEntity
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        public JsonStorage()
        {
            _filePath = Path.Combine("Data", $"{typeof(T).Name}.json");

            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new UserConverter() }
            };

            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(_filePath))
                {
                    File.WriteAllText(_filePath, "[]");
                }
                else if (new FileInfo(_filePath).Length == 0)
                {
                    File.WriteAllText(_filePath, "[]");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize JSON file at {_filePath}", ex);
            }
        }

        public void Save(List<T> items)
        {
            try
            {
                var json = JsonSerializer.Serialize(items, _options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save data to {_filePath}", ex);
            }
        }

        public List<T> Load()
        {
            try
            {
                var json = File.ReadAllText(_filePath);

                
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<T>();
                }

                var result = JsonSerializer.Deserialize<List<T>>(json, _options);
                return result ?? new List<T>();
            }
            catch (JsonException)
            {
                
                return new List<T>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load data from {_filePath}", ex);
            }
        }
        public class UserConverter : JsonConverter<User>
        {
            public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    var root = doc.RootElement;

                    
                    if (root.TryGetProperty("Role", out var roleElement))
                    {
                        var role = roleElement.GetString();
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
    }
}