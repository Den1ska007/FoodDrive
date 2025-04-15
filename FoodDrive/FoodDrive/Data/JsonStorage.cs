// FoodDrive/Data/JsonStorage.cs
using System.Text.Json;
using FoodDrive.Interfaces;
using System.IO;

namespace FoodDrive.Data
{
    public class JsonStorage<T> : IDataStorage<T> where T : IEntity
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;
        public JsonStorage()
        {
            _filePath = Path.Combine("Data", $"{typeof(T).Name}.json");
            _options = new JsonSerializerOptions
            {
                Converters = { new UserConverter() },  // Додаємо конвертер
                WriteIndented = true
            };
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            // Створюємо папку Data, якщо її немає
            Directory.CreateDirectory("Data");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public void Save(List<T> items)
        {
            var json = JsonSerializer.Serialize(items, _options);
            File.WriteAllText(_filePath, json);
        }

        public List<T> Load()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
        }
    }
}