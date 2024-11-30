using ApplicationCore.Enums;

namespace api.Models.Contragents
{
    public class ContragentFilterModel
    {
        public string? Name { get; set; }
        public bool IsAlive { get; set; }
        public Countries? Country { get; set; }
        public string? INN { get; set; }
        public int ItemsPerPage { get; set; } = 20;
        public bool? DeliveryPoint { get; set; }
        public bool? ClientCompany { get; set; }
        public bool? LogisticCompany { get; set; }
        public int Page { get; set; } = 1;
        public bool NotActiveFilter { get; set; } = false;
    }
}
