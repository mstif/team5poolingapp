
using ApplicationCore.Documents;
using Services.Contracts;

namespace ApplicationCore.Interfaces.Documents
{
    public interface IOrderRepository : IDocumentRepository<Order> 
    {
        public Order? GetLastOrder();
        Task<List<Order>> GetPagedAsync(OrderFilterDto filterDto);
        Task<List<Order>> GetPagedDashboardAsync(OrderFilterDto filterDto);
    }
}
