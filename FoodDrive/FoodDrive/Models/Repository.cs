// FoodDrive/Models/Repository.cs
using FoodDrive.Interfaces;
using FoodDrive.Data;
using System.Linq;

namespace FoodDrive.Models
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private List<T> _entities;
        private readonly IDataStorage<T> _storage;

        public Repository(IDataStorage<T> storage)
        {
            _storage = storage;
            _entities = _storage.Load();
        }

        public void Add(T entity)
        {
            if (!_entities.Any(e => e.id == entity.id))
            {
                _entities.Add(entity);
                _storage.Save(_entities);
            }
        }

        public void Remove(T entity)
        {
            _entities.Remove(entity);
            _storage.Save(_entities);
        }

        public T GetById(int id) => _entities.FirstOrDefault(e => e.id == id);
        public IEnumerable<T> GetAll() => _entities.ToList();
        public IEnumerable<T> GetSorted() => _entities.OrderBy(e => e.id).ToList();
    }
}