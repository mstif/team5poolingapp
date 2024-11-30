using System.Security.Policy;

namespace api.Models.Orders
{
    public class OrderRedisModel
    {

        public string? Status { get; set; }
        public long OrderId { get; set; }
        
    }
}
