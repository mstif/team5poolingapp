using Services.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IOffersService
    {

        bool MakeOfferToSeller(LogisticOffer offer, IOrderService orderService);
        Task SetAutoCost(IOrderService orderService,
           IContragentService contragentService,
           ILogisticPriceService logisticPriceService,
           long? currentLogistId = null,
           long? currentOrderId = null);
        bool AgreedSellerOffer(LogisticOffer offer);
        bool RemoveOfferDeliveryLogist(LogisticOffer offer);
        bool RemoveOfferDeliveryOrder(long orderId);
        Task<ICollection<LogisticOffer>?> GetBestOffersAsync(OrderDto order);
        Task<LogisticOffer?> GetOfferAsync(string Offerid);
        Task<LogisticOffer?> GetOfferAsync(long orderId,long logistid);
        Task SetNewCostDelivery(decimal newCost, long orderId, long logistId);
        Task ClearData();

    }
}
