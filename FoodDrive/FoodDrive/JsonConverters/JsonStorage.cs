using System.Text.Json;
using FoodDrive.Interfaces;
using System.IO;
using System;
using FoodDrive.Models;
using System.Text.Json.Serialization;

namespace FoodDrive.JsonConverters
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
                Converters = { new UserConverter() } // Для обробки успадкування
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
                Console.WriteLine($"Помилка ініціалізації файлу: {ex.Message}");
            }
        }

        public List<T> Load()
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
            }
            catch (JsonException)
            {
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження {_filePath}: {ex.Message}");
                return new List<T>();
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
                Console.WriteLine($"Помилка збереження {_filePath}: {ex.Message}");
            }
        }
    }
}