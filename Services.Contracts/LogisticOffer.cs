using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.OM.Modeling;
using Services.Contracts;
namespace Services.Contracts
{

    [Document(StorageType = StorageType.Json, Prefixes = new[] { "LogisticOffer" })]
    public class LogisticOffer
    {
        [RedisIdField][Indexed] public string? OfferId  => LogisticCompanyId+"_"+ OrderId;
        [Indexed] public string LogisticCompanyId { get; set; }=string.Empty;
        [Indexed] public long OrderId { get; set; } = 0;
        [Indexed(CascadeDepth = 1)] public ContragentDto LogisticCompany { get; set; } = null!;
       // [Indexed(CascadeDepth = 1)] public OrderDto Order { get; set; } = null!;
        [Indexed] public decimal Amount {  get; set; }
        [Indexed] public bool isAuto {  get; set; }
        [Indexed] public bool LogistAgreed { get; set; } = false;
        [Indexed] public bool SellerAgreed { get; set; } = false;


    }
    [Document(StorageType = StorageType.Json,Prefixes = new[] { "LogisticOfferSet" })]
    public class LogisticOfferSet
    {
        [Indexed][RedisIdField] public string OrderId { get; set; }
        [Indexed(CascadeDepth = 1)] public List<LogisticOffer>? LogistOffers { get; set; }
 
    }

   
}
