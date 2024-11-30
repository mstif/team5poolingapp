using ApplicationCore.Interfaces.Books;
using ApplicationCore.Interfaces.Documents;
using ApplicationCore.Interfaces.Registries;

namespace ApplicationCore.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICargoTypeRepository CargoTypes { get; }
        IContragentRepository Contragents { get; }
        IOrderRepository Orders { get; }
        IInvoiceRepository Invoices { get; }
        INumeratorRepository Numerators { get; }
        ILogisticPriceRepository LogisticPrices { get; }
        IDeliveryContractRepository DeliveryContracts { get; }
        public void SaveChanges();
        public Task SaveChangesAsync();
    }
}
