using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationUsers;
namespace Services.Abstractions
{
    public interface IDocumentService<T> where T : class
    {
        IEnumerable<T> GetDocumentsList();
        Task<List<T>> GetDocumentsListAsync();
        T? GetDocumentById(long? id);
        Task<T?> GetDocumentByIdAsync(long? id);
        T Add(T item);
        Task<T> AddAsync(T item);
        void AddRange(List<T> item);
        Task AddRangeAsync(ICollection<T> items);
        T? Update(T item);
        bool Delete(long id);
        Task<T?> GetDocumentByNumber(string number,int year);
        void Save();
        Task SaveAsync();
        Task<T> OpenDocument(long? id);
        Task<T> CreateDocument();
        Task<T> SaveDocument(T dto);
        bool AccessDocumentForView(T item, UserInfo user);
        bool AccessDocumentForEdit(T item, UserInfo user);

    }
}
