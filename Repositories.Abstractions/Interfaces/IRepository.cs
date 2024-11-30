using ApplicationCore.Entities.Base;

namespace ApplicationCore.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetItemsList(bool asNoTracking);
        Task<List<T>> GetItemsListAsync(bool asNoTracking);
        T? GetItemById(long? id);
        Task<T?> GetItemByIdAsync(long? id);
        IEnumerable<T> Find(Func<T, Boolean> predicate);
        Task<IEnumerable<T>> FindAsync(Func<T, Boolean> predicate);
        T Add(T item);
        Task<T> AddAsync(T item);
        void AddRange(List<T> item);
        Task AddRangeAsync(ICollection<T> items);
        T Update(T item);
        bool Delete(long id);
        void Save();
        Task SaveAsync();
    }
}
