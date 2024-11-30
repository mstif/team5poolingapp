
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public class OrderDto
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public DateTime? Date{ get; set; }
        public bool IsAlive { get; set; }
        public string Number { get; set; }
        public string? Title => "Заказ " + Number + " от " + Date.Value.ToString("dd.MM.yyyy HH:MM");
        public long? SellerId { get; set; }
       
        public virtual ContragentDto? Seller { get; set; }

        public long? LogisticCompanyId { get; set; }

        public virtual ContragentDto? LogisticCompany { get; set; }


        public long? CargoTypeId { get; set; }


        public virtual CargoTypeDto? CargoType { get; set; }


        public string? Status { get; set; } 

        public DateTime? DateDeparture { get; set; } = DateTime.Now.AddDays(1);

        public virtual List<InvoiceDto>? Invoices { get; set; }
       
        public virtual string? Author { get; set; }


        public DateTime? DateCreated { get; set; }

        public int TotalDistance {  get; set; }
        public int TotalTime { get; set; }
    }
}
