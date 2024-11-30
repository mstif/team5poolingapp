using ApplicationCore.Documents;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Documents
{
    public interface IInvoiceRepository : IDocumentRepository<Invoice>
    {
        Task<List<Invoice>> GetPagedAsync(InvoiceFilterDto filterDto);
    }
}
