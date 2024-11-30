
using ApplicationCore.Base;
using ApplicationCore.Enums;
using System.ComponentModel;

namespace ApplicationCore.Books
{
    public class Contragent : EntityBook
    {

        public Countries Country { get; set; } = Countries.Russia;
        public string? INN { get; set; }
        public bool DeliveryPoint { get; set; } = false;
        public bool ClientCompany { get; set; } = false;
        public bool LogisticCompany { get; set; } = false;
        public string? Latitude {  get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
    
    }
}
