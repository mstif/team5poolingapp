using ApplicationCore.Books;
using ApplicationCore.Interfaces.Books;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace Repositories.Implementation.Repositories.Books
{
    public class ContragentRepository : BookRepository<Contragent>, IContragentRepository
    {

        private ApplicationDbContext db;
        public ContragentRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }

        public async Task<List<Contragent>> GetPagedAsync(ContragentFilterDto filterDto)
        {
            var query = this.GetItemsList().Where(c => c.IsAlive).OrderBy(o => o.Name).AsQueryable();
            var res = query.ToList();
            if (filterDto != null && !filterDto.NotActiveFilter)
            {
                if (filterDto.Name != null) query = query.Where(c => c.Name.ToLower().Contains(filterDto.Name.ToLower()) );
                if (filterDto.Country != null && filterDto.Country != 0) query = query.Where(c => c.Country == filterDto.Country);
                if (filterDto.INN != null) query = query.Where(c => c.INN == null ? false : c.INN.Contains(filterDto.INN));
                if (filterDto.LogisticCompany != null) query = query.Where(c => c.LogisticCompany == filterDto.LogisticCompany);
                if (filterDto.ClientCompany != null) query = query.Where(c => c.ClientCompany == filterDto.ClientCompany);
                if (filterDto.DeliveryPoint != null) query = query.Where(c => c.DeliveryPoint == filterDto.DeliveryPoint);
                res = query.ToList();
                query = query
                    .Skip((filterDto.Page - 1) * filterDto.ItemsPerPage)
                    .Take(filterDto.ItemsPerPage);
            }
            res = query.ToList();
            return await query.ToListAsync();
        }
    }
}
