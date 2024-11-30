using ApplicationCore.Books;
using ApplicationCore.Interfaces.Books;
using Infrastructure.EntityFramework;

namespace Repositories.Implementation.Repositories.Books
{
    public class CargoTypesRepository : BookRepository<CargoType>, ICargoTypeRepository
    {
        private ApplicationDbContext db;

        public CargoTypesRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }
    }
}
