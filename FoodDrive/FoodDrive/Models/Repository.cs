
namespace FoodDrive.Models;
using FoodDrive.Interfaces;

public class Repository<T> : IRepository<T> where T : IEntity
{
    protected static List<T> _entities = new List<T>(); // 🟢 Статичний список зберігає дані між запитами

    public void Add(T entity)
    {
        if (!_entities.Any(e => e.id == entity.id)) // 🟢 Перевіряємо унікальність ID
        {
            _entities.Add(entity);
        }
    }

    public void Remove(T entity) => _entities.Remove(entity);
    public T GetById(int id) => _entities.FirstOrDefault(e => e.id == id);
    public IEnumerable<T> GetAll() => _entities.ToList(); // 🟢 Повертаємо копію списку
    public IEnumerable<T> GetSorted() => _entities.OrderBy(e => e.id).ToList(); // 🟢 Сортуємо список
}