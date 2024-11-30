using System.Security.Policy;

namespace api.Models.Orders
{
    public class OrderFilterModel
    {
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public int ItemsPerPage { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; }
        public bool NotActiveFilter {  get; set; } = false;
    }
}
