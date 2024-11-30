using ApplicationCore.Enums;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationUsers;
namespace Services.Abstractions
{
    public interface IInvoiceService:IDocumentService<InvoiceDto>
    {
        Task<ICollection<InvoiceDto>> GetPagedAsync(InvoiceFilterDto filterDto);

        Task<ICollection<InvoiceDto>> GetPagedListForUserAsync(UserInfo userInfo, InvoiceFilterDto filterDto);

        Task<InvoiceDto?> SetStatusToInvoice(UserInfo userInfo, InvoiceDto invoiceDto, InvoiceStatus status);

    }
}
