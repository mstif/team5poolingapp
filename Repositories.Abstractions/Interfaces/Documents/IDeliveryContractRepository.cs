using ApplicationCore.Documents;
using Services.Contracts;

namespace ApplicationCore.Interfaces.Documents
{
    public interface IDeliveryContractRepository : IDocumentRepository<DeliveryContract>
    {
        Task<List<DeliveryContract>> GetPagedAsync(DeliveryContractFilterDto filterDto);
    }
}
