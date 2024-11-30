using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Books;
using ApplicationCore.Interfaces.Documents;
using ApplicationCore.Interfaces.Registries;
using Infrastructure.EntityFramework;
using Repositories.Implementation.Repositories.Books;
using Repositories.Implementation.Repositories.Documents;
using Repositories.Implementation.Repositories.Registries;

namespace App.Domain.Core.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;

        private ContragentRepository? contragentRepository;
        private CargoTypesRepository? cargoTypesRepository;
        private InvoiceRepository? invoiceRepository;
        private OrderRepository? orderRepository;
        private NumeratorRepository? numeratorRepository;
        private LogisticPriceRepository? logisticPriceRepository;
        private DeliveryContractRepository? deliveryContractRepository;

        public EFUnitOfWork(ApplicationDbContext context)
        {

            db = context;
        }

        public IContragentRepository Contragents
        {
            get
            {
                if (contragentRepository == null)
                    contragentRepository = new ContragentRepository(db);
                return contragentRepository;
            }
        }
        public ICargoTypeRepository CargoTypes
        {
            get
            {
                if (cargoTypesRepository == null)
                    cargoTypesRepository = new CargoTypesRepository(db);
                return cargoTypesRepository;
            }
        }


        public IOrderRepository Orders
        {
            get
            {
                if (orderRepository == null)
                    orderRepository = new OrderRepository(db);
                return orderRepository;
            }
        }

        public IInvoiceRepository Invoices
        {
            get
            {
                if (invoiceRepository == null)
                    invoiceRepository = new InvoiceRepository(db);
                return invoiceRepository;
            }
        }

        public INumeratorRepository Numerators
        {
            get
            {
                if (numeratorRepository == null)
                    numeratorRepository = new NumeratorRepository(db);
                return numeratorRepository;
            }
        }

        public ILogisticPriceRepository LogisticPrices
        {
            get
            {
                if (logisticPriceRepository == null)
                    logisticPriceRepository = new LogisticPriceRepository(db);
                return logisticPriceRepository;
            }
        }

        public IDeliveryContractRepository DeliveryContracts
        {
            get
            {
                if (deliveryContractRepository == null)
                    deliveryContractRepository = new DeliveryContractRepository(db);
                return deliveryContractRepository;
            }
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
