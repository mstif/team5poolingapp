
using ApplicationCore.Base;
using ApplicationCore.Books;
using System.ComponentModel.DataAnnotations.Schema;
namespace ApplicationCore.Documents
{
    public class DeliveryContract : EntityDocument
    {
        public long? LogisticCompanyId { get; set; }

        [ForeignKey("LogisticCompanyId")]
        public virtual Contragent? LogisticCompany { get; set; }

        public Contragent? Seller  => Order?.Seller;

        public DateTime? DateDelivery { get; set; }

        public decimal TotalCostDelivery { get; set; }

        public string Status { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        public long? OrderId { get; set; }
        public DeliveryContract() { }
    }
}
