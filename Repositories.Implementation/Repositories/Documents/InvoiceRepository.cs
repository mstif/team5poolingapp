
using ApplicationCore.Documents;
using ApplicationCore.Interfaces.Documents;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.Repositories.Documents
{

    public class InvoiceRepository : DocumentRepository<Invoice>,IInvoiceRepository
    {
        private ApplicationDbContext db;
        public InvoiceRepository(ApplicationDbContext context):base(context) 
        {
            db = context;
        }
        public async Task<List<Invoice>> GetPagedAsync(InvoiceFilterDto filterDto)
        {
            var query = this.GetItemsList().Where(c => c.IsAlive).OrderBy(o => o.Date).AsQueryable();
            //.Include(c => c.Lessons).AsQueryable();
            if (filterDto != null && !filterDto.NotActive)
            {
                if ((filterDto.DateStart != null && filterDto.DateEnd != null))
                {
                    query = query.Where(c => c.Date >= filterDto.DateStart && c.Date <= filterDto.DateEnd);
                }

                if (filterDto.OrderId != 0)
                {
                    query = query.Where(c => c.OrderId == filterDto.OrderId);
                }
                if (filterDto.Status != default)
                {
                    query = query.Where(c => c.StatusDoc == filterDto.Status);
                }

                query = query
                    .Skip((filterDto.Page - 1) * filterDto.ItemsPerPage)
                    .Take(filterDto.ItemsPerPage);
            }


            return await query.ToListAsync();
        }

        public override Invoice Update(Invoice document)
        {
            db.Entry(document).State = EntityState.Modified;
            db.Entry(document).Property(p => p.Number).IsModified = false;
            return db.Entry(document).Entity;
        }

    }
}
