using api.Models;
using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public class InvoiceModel
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public DateTime? Date { get; set; }
        public bool IsAlive { get; set; }
        public string Number { get; set; } = string.Empty;
        public string? Title { get; set; } 
        public DateTime? DateDeliveryFrom { get; set; }
        public DateTime? DateDeliveryUpTo { get; set; }

        public long? DeliveryPointId { get; set; }

        public  EntityModel? DeliveryPoint { get; set; }

        public int PallettAmount { get; set; }
        public InvoiceStatus StatusDoc { get; set; } = InvoiceStatus.New;
        public int BoxAmount { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalCost { get; set; }

        public EntityModel? OrderModel { get; set; }

        public long? OrderId { get; set; }
    }
}
