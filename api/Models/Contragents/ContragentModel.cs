using ApplicationCore.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace api.Models.Contragents
{
    public class ContragentModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public bool IsAlive { get; set; } = true;
        public Countries Country { get; set; }=Countries.Russia;
        public string? INN { get; set; }
        public bool DeliveryPoint { get; set; }
        public bool ClientCompany { get; set; }
        public bool LogisticCompany { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public List<Countries> AllCountries => Enum.GetValues(typeof(Countries)).Cast<Countries>().ToList();

    }
    
}
