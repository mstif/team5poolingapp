using ApplicationCore.Entities.Books;
using ApplicationCore.Interfaces.Books;
using Infrastructure.EntityFramework;

namespace Repositories.Implementation.Repositories.Books
{
    internal class LogisticPriceRepository : BookRepository<LogisticPrice>, ILogisticPriceRepository
    {
        private ApplicationDbContext db;
        public LogisticPriceRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }
    }
}
