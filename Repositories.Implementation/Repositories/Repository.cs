using ApplicationCore.Entities.Base;
using ApplicationCore.Interfaces;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Implementation
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity
    {

        private readonly ApplicationDbContext db;
        private readonly DbSet<T> _entitySet;
        public Repository(ApplicationDbContext context)
        {
            this.db = context;
            _entitySet = db.Set<T>();

        }

        public virtual  T Add(T item)
        {
            var objToReturn = _entitySet.Add(item);
            //db.SaveChanges();
            return objToReturn.Entity;
        }

        public virtual async Task<T> AddAsync(T items)
        {
            return (await _entitySet.AddAsync(items)).Entity;
        }

        public virtual void AddRange(List<T> items)
        {
            var enumerable = items as IList<T> ?? items.ToList();
            _entitySet.AddRange(enumerable);
        }

        public virtual async Task AddRangeAsync(ICollection<T> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }
            await _entitySet.AddRangeAsync(items);
        }

        public virtual bool Delete(long id)
        {
            T? item = _entitySet.Find(id);
            if (item != null)
            {
                _entitySet.Remove(item);
                return true;
            }
            return false;
        }

        public virtual IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _entitySet.Where(predicate).ToList() ?? Enumerable.Empty<T>();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            var setData = _entitySet.Where(predicate);
            if (setData.Count() > 0)
                return await Task.Run(() => setData.ToList());
            else
                return new List<T>();
        }

        public virtual T? GetItemById(long? id)
        {
            return _entitySet.AsNoTracking().FirstOrDefault(i => i.Id == id);
        }

        public virtual async Task<T?> GetItemByIdAsync(long? id)
        {
            return await _entitySet.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }


        public virtual IQueryable<T> GetItemsList(bool asNoTracking = true)
        {
            return asNoTracking ? _entitySet.AsNoTracking() : _entitySet;
        }

        public virtual async Task<List<T>> GetItemsListAsync(bool asNoTracking = true)
        {
            return await GetItemsList(asNoTracking).ToListAsync();
        }

        public virtual void Save()
        {
            db.SaveChanges();
        }

        public virtual async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public virtual T Update(T item)
        {
            db.Entry(item).State = EntityState.Modified;
            return db.Entry(item).Entity;
        }
    }
}
