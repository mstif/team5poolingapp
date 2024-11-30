using ApplicationCore.Base;
using ApplicationCore.Interfaces.Documents;
using Infrastructure.EntityFramework;

namespace Repositories.Implementation.Repositories.Documents
{
    public class DocumentRepository<T> : Repository<T>, IDocumentRepository<T> where T : EntityDocument
    {
        private readonly ApplicationDbContext db;
        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }

        public T? GetDocumentByNumber(string number)
        {          
                return db.Set<T>().FirstOrDefault(o => o.Number == number);
        }

        public void PrintDocument()
        {
            throw new NotImplementedException();
        }
    }
}

