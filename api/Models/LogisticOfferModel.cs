using Redis.OM.Modeling;
using Services.Contracts;

namespace api.Models
{
    public class LogisticOfferModel
    {
        public string LogisticCompanyId { get; set; } = string.Empty;
       // public string LogisticCompanyName { get; set; } = string.Empty;
        public long OrderDtoId { get; set; } = 0;

        public decimal Amount { get; set; }
        public bool isAuto { get; set; }
    }
}
