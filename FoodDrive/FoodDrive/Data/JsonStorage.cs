// FoodDrive/Data/JsonStorage.cs
using System.Text.Json;
using FoodDrive.Interfaces;
using System.IO;

namespace FoodDrive.Data
{
    public class JsonStorage<T> : IDataStorage<T> where T : IEntity
    {
        private readonly string _filePath;

        public JsonStorage()
        {
            // Використовуємо тип T для створення унікального імені файлу
            _filePath = Path.Combine("Data", $"{typeof(T).Name}.json");
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(items, options);
            File.WriteAllText(_filePath, json);
        }

        public List<T> Load()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}