using Services.Contracts;

namespace Services.Abstractions
{
    public interface IDeliveryContractService : IDocumentService<DeliveryContractDto>
    {
        Task<ICollection<DeliveryContractDto>> GetPagedAsync(DeliveryContractFilterDto filterDto);
        DeliveryContractDto GetContractForOrder(long OrderId);
    }
}
