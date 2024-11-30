using api.Models.Contragents;

namespace api.Models
{
    public class EntityInvoiceModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string Number {  get; set; }
        public string Date { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalCost { get; set; }
        public ContragentModel DeliveryPoint { get; set; }
    }
}


   
