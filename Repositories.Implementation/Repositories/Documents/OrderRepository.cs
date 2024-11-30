
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
    public class OrderRepository : DocumentRepository<Order>, IOrderRepository
    {
        private ApplicationDbContext db;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            db = context;
        }

        public Order? GetLastOrder()
        {
            return db.Set<Order>().LastOrDefault();
        }

        public async Task<List<Order>> GetPagedAsync(OrderFilterDto filterDto)
        {
            var query = this.GetItemsList().Where(c => c.IsAlive).OrderBy(o => o.Date).AsQueryable();
            //.Include(c => c.Lessons).AsQueryable();
            if (filterDto != null && !filterDto.NotActiveFilter)
            {
                if ((filterDto.OrderDateStart != null && filterDto.OrderDateEnd != null))
                {
                    query = query.Where(c => c.Date >= filterDto.OrderDateStart && c.Date <= filterDto.OrderDateEnd);
                }
                query = query
                    .Skip((filterDto.Page - 1) * filterDto.ItemsPerPage)
                    .Take(filterDto.ItemsPerPage);
            }


            return await query.ToListAsync();
        }
        public async Task<List<Order>> GetPagedDashboardAsync(OrderFilterDto filterDto)
        {
            var query = this.GetItemsList().Where(c => c.IsAlive).OrderBy(o => o.Date).AsQueryable();
            //.Include(c => c.Lessons).AsQueryable();
            if (filterDto != null && !filterDto.NotActiveFilter)
            {


                query = query.Where(c => c.Status != "Завершен" && c.Status != "Черновик");
                query = query
                    .Skip((filterDto.Page - 1) * filterDto.ItemsPerPage)
                    .Take(filterDto.ItemsPerPage);
            }


            return await query.ToListAsync();
        }


        public override Order Update(Order document)
        {
            db.Entry(document).State = EntityState.Modified;
            db.Entry(document).Property(p => p.Number).IsModified = false;
            return db.Entry(document).Entity;
        }
    }
}
