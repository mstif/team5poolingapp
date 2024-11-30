using ApplicationCore.Base;
using ApplicationCore.Books;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities.Books
{
    public class LogisticPrice : EntityBook
    {
    
        public DateTime StartDate { get; set; }

        public decimal CostPerTnKm { get; set; }

        public decimal CostStart { get; set; }

        public long? LogisticCompanyId { get; set; }

        [ForeignKey("LogisticCompanyId")]
        public virtual Contragent? LogisticCompany { get; set; }

    }
}
