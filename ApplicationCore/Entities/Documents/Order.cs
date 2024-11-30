
using ApplicationCore.Base;
using ApplicationCore.Books;
using ApplicationCore.Documents;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Documents
{
    public class Order : EntityDocument
    {
        public long? SellerId { get; set; }
        [ForeignKey("SellerId")]
        public virtual Contragent? Seller { get; set; }

        public long? LogisticCompanyId { get; set; }

        [ForeignKey("LogisticCompanyId")]
        public virtual Contragent? LogisticCompany { get; set; }

      
        public long? CargoTypeId { get; set; }

        [ForeignKey("CargoTypeId")]
    
        public virtual CargoType? CargoType { get; set; }


        public string? Status { get; set; } = "Новый";
        
        public DateTime? DateDeparture { get; set; }

        public virtual List<Invoice>? Invoices { get; set; }
        public virtual string? Author { get; set; }

        public DateTime? DateCreated { get; set; }

        public int TotalDistance {  get; set; }

        public int TotalTime { get; set; }

        public Order() { }
    }
}
