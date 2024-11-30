using ApplicationUsers;
using Services.Contracts;

namespace Services.Abstractions
{
    public interface IOrderService : IDocumentService<OrderDto>
    {
        Task<ICollection<OrderDto>> GetPagedAsync(OrderFilterDto filterDto);

        Task<ICollection<OrderDto>> GetPagedListForUserAsync(UserInfo userInfo, OrderFilterDto filterDto);
        Task<ICollection<OrderDto>> GetPagedListDashboardForUserAsync(OrderFilterDto filterDto);
        public Task<OrderDto?> MakeOfferToLogisticCompany(OrderDto orderDto, long logist);
        public Task<OrderDto?> CancelOfferToLogisticCompany(OrderDto orderDto);

        Task<OrderDto?> SetStatusToOrder(UserInfo userInfo, OrderDto orderDto, string status);

       // ICollection<LogisticOffer> GetBestOffersAsync( OrderDto order);

        Task<decimal> GetDistanceTotalAsync(OrderDto order);

        Task<decimal> GetTimeTotalAsync(OrderDto order);
        Task<OrderDto?> RefuseOrder(OrderDto orderDto, long logist);
        Task TakeOrderToWork(OrderDto orderDto, long logistId, LogisticOffer offer);
        Task<List<OrderDto>> GetActiveOrderListAsync();
        Task<List<PairValue>> GetAvailableActions(OrderDto order);
        Task SetAutoCost(long? currentLogistId = null, long? currentOrderId = null);
    }
}
