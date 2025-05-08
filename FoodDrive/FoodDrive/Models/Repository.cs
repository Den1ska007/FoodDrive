// FoodDrive/Models/Repository.cs
using FoodDrive.Interfaces;
using FoodDrive.JsonConverters;
using System.Linq;

namespace FoodDrive.Models
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        protected List<T> _entities;
        protected readonly IDataStorage<T> _storage;

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
        public void Update(T entity)
        {
            var existingEntity = _entities.FirstOrDefault(e => e.id == entity.id);
            if (existingEntity != null)
            {
                _entities.Remove(existingEntity);
                _entities.Add(entity);
                _storage.Save(_entities);
            }
        }
        public T GetById(int id) => _entities.FirstOrDefault(e => e.id == id);
        public IEnumerable<T> GetAll() => _entities.ToList();
        public IEnumerable<T> GetSorted() => _entities.OrderBy(e => e.id).ToList();
    }
}