using ApplicationCore.Enums;
using Redis.OM.Modeling;

namespace Services.Contracts
{
    public class ContragentFilterDto
    {
        public string? Name { get; set; }
        public bool IsAlive { get; set; }
        public Countries? Country { get; set; }
        public string? INN { get; set; }
        public int ItemsPerPage { get; set; } = 20;
        public int Page { get; set; } = 1;
        public bool? DeliveryPoint { get; set; }
        public bool? ClientCompany { get; set; }
        public bool? LogisticCompany { get; set; }
        public bool NotActiveFilter { get; set; } = false;
    }
}
