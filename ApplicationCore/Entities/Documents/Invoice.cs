
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Base;
using ApplicationCore.Books;
using ApplicationCore.Enums;
namespace ApplicationCore.Documents
{
    public class Invoice : EntityDocument
    {


        public DateTime? DateDeliveryFrom { get; set; }
        public DateTime? DateDeliveryUpTo { get; set; }

        public long? DeliveryPointId {  get; set; }

        [ForeignKey("DeliveryPointId")]
        public virtual Contragent? DeliveryPoint { get; set; }

        public int PallettAmount { get; set; }
        public InvoiceStatus StatusDoc { get; set; } = InvoiceStatus.New;
        public int BoxAmount { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalCost { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
       
        public long OrderId { get; set; }
        public Invoice() { }
    }
}
