using ApplicationCore.Books;

namespace api.Models.LogisticPrice
{
    public class LogisticPriceModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public bool IsAlive { get; set; }
        public DateTime StartDate { get; set; }
        public decimal CostPerTnKm { get; set; }
        public decimal CostStart { get; set; }
        public long? LogisticCompanyId { get; set; }
        public Contragent? LogisticCompany { get; set; }
    }
}
