using ApplicationCore.Base;
using ApplicationCore.Interfaces.Books;
using Infrastructure.EntityFramework;

namespace Repositories.Implementation.Repositories.Books
{
    public class BookRepository<T> : Repository<T>, IBookRepository<T> where T : EntityBook
    {
        private readonly ApplicationDbContext db;
        public BookRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }
    
    }
}
