using api.Models.DeliveryContract;
using ApplicationUsers;
using Services.Abstractions;
using Services.Contracts;

namespace api.Models.Orders
{
    public class OrderModel
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public DateTime? Date { get; set; }
        public bool IsAlive { get; set; }
        public string? Number { get; set; }
        public string? Title { get; set; }
        public virtual EntityModel? Seller { get; set; }
        public virtual EntityModel? LogisticCompany { get; set; }
        public virtual EntityModel? CargoType { get; set; }
        public string? Status { get; set; }
        public DateTime? DateDeparture { get; set; }
        public virtual List<EntityInvoiceModel>? Invoices { get; set; }
        public virtual string? Author { get; set; }
        public DateTime? DateCreated { get; set; }
        public int TotalDistance { get; set; }
        public int TotalTime { get; set; }
        public decimal TotalWeight { get; set; }
        public List<PairValue> AvailableActions {  get; set; }=new List<PairValue>();
        public List<LogisticOffer>? LogisticOffers { get; set; }=new List<LogisticOffer>();
        public DeliveryContractDto? DeliveryContract { get; set; }
    }
}
