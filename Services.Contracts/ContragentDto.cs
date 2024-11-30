using ApplicationCore.Enums;
using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "ContragentDto" })]
    public class ContragentDto
    {
        [RedisIdField][Indexed] public long Id { get; set; }
        [Indexed] public string? Name { get; set; }
        [Indexed] public string? Comment { get; set; }
        [Indexed] public bool IsAlive { get; set; }
        [Indexed] public Countries Country { get; set; }
        [Indexed] public string? INN { get; set; }
        [Indexed] public bool DeliveryPoint { get; set; }
        [Indexed] public bool ClientCompany { get; set; }
        [Indexed] public bool LogisticCompany { get; set; }
        [Indexed] public string? Latitude { get; set; }
        [Indexed] public string? Longitude { get; set; }
        [Indexed] public string? Address { get; set; }
    }
}
