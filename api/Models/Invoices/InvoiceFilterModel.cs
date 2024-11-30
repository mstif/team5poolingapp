using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public class InvoiceFilterModel
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public InvoiceStatus? Status {  get; set; }
        public long OrderId { get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public bool NotActive { get; set; } = false;
    }
}
