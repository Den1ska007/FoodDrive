namespace FoodDrive.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Remove(T entity);
        void Update(T entity);
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetSorted();
    }
}

