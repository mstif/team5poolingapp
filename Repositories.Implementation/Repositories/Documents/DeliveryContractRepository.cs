using ApplicationCore.Documents;
using ApplicationCore.Interfaces.Documents;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace Repositories.Implementation.Repositories.Documents
{
    internal class DeliveryContractRepository : DocumentRepository<DeliveryContract>, IDeliveryContractRepository
    {
        private ApplicationDbContext db;
        public DeliveryContractRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }

        public async Task<List<DeliveryContract>> GetPagedAsync(DeliveryContractFilterDto filterDto)
        {
            var query = this.GetItemsList().Where(c => c.IsAlive).OrderBy(o => o.Date).AsQueryable();
            if (filterDto != null && !filterDto.NotActive)
            {
                if (filterDto.Status != null) query = query.Where(c => c.Status == filterDto.Status);
                if (filterDto.Title != null) query = query.Where(c => c.Title.ToLower().Contains(filterDto.Title.ToLower()));

                query = query
                    .Skip((filterDto.Page - 1) * filterDto.ItemsPerPage)
                    .Take(filterDto.ItemsPerPage);
            }

            return await query.ToListAsync();
        }
    }
}