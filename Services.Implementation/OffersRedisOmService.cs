using ApplicationCore.Documents;
using Redis.OM.Searching;
using Redis.OM;
using Services.Abstractions;
using Services.Contracts;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Services.Implementation
{
    public class OffersRedisOmService : IOffersService
    {
        private RedisCollection<LogisticOffer> _offers;
        private readonly RedisConnectionProvider _provider;


        public OffersRedisOmService(RedisConnectionProvider provider)
        {
            _provider = provider;
            _offers = (RedisCollection<LogisticOffer>)provider.RedisCollection<LogisticOffer>();

        }



        public async Task<ICollection<LogisticOffer>?> GetBestOffersAsync(OrderDto order)
        {
    
            var offers = _offers.Where(o => o.OrderId == order.Id);
            var result = offers?.OrderBy(o => o.Amount).ToList();
            return result;
        }

        public async Task SetNewCostDelivery(decimal newCost, long orderId, long logistId)
        {
            var offer = _offers?.Where(o => o.OrderId == orderId && o.LogisticCompany.Id == logistId).FirstOrDefault();
            if (offer != null)
            {
                offer.Amount = newCost;
                _offers?.Update(offer);
            }

        }

        public async Task ClearData()
        {   if (_offers != null)
            {
                var allElements = _offers!.ToList();

                await _offers!.DeleteAsync(allElements);
            }
        }
        public async Task SetAutoCost(
            IOrderService orderService,
            IContragentService contragentService,
            ILogisticPriceService logisticPriceService,
            long? currentLogistId = null,
            long? currentOrderId = null)
        {

            var allLogists = (await contragentService.GetPagedAsync(new ContragentFilterDto() { NotActiveFilter = true }))
                .Where(c => c.LogisticCompany && (currentLogistId == null || c.Id == currentLogistId)).ToList();

            var allOrders = (await orderService.GetActiveOrderListAsync()).Where(o => o.Status == "Новый"
            && (currentOrderId == null || o.Id == currentOrderId));

            foreach (var order in allOrders)
            {
                var offers = _offers?.Where(o => o.OrderId == order.Id);



                foreach (var logist in allLogists)
                {
                    var logistId = logist.Id.ToString();
                    var cnt = offers?.Count();
                    var finded = offers?.FirstOrDefault(o => o.LogisticCompanyId == logistId);
                    var price = await logisticPriceService.GetLastPriceLogist(logist.Id);//new LogisticPriceDto() { CostPerTnKm = 1255, CostStart = 4588, LogisticCompanyId = logist.Id };//(await logisticPriceService.GetBookByIdAsync(0));
                    if (price.Id == 0) { continue; }
                    var cost = (price?.CostStart ?? 0) + (price?.CostPerTnKm ?? 0) * (order.TotalDistance/1000m) * (order.Invoices?.Sum(i => i.Weight) ?? 0)/1000m;
                    var newoffer = new LogisticOffer
                    {
                        LogisticCompany = logist,
                        LogisticCompanyId = logist.Id.ToString(),
                        OrderId = order.Id,
                        Amount = cost,
                        isAuto = true
                    };
                    if (finded == null || finded.LogisticCompanyId == "")
                    {
                        await _offers.InsertAsync(newoffer);
                    }
                    else
                    {
                        foreach (LogisticOffer obj in offers!)
                        {
                            if (obj.LogisticCompany.Id == logist.Id && obj.isAuto)
                            {
                                obj.Amount = cost;
                                offers?.SaveAsync();
                                break;
                            }

                        }

                    }

                }

            }
        }

        public bool MakeOfferToSeller(LogisticOffer offer, IOrderService orderService)
        {
            bool result = false;
            var offers = _offers.Where(o => o.OrderId == offer.OrderId);

            offer.LogistAgreed = true;
            if (offers.Count() == 0)
            {
                var order = orderService.GetDocumentById(offer.OrderId);
                if (order?.Status == "Новый")
                {
                    offers.Insert(offer);
                    return true;
                }
                return false;
            }
            var obj = offers.FirstOrDefault(o => o.LogisticCompanyId == offer.LogisticCompanyId);
            if (obj == null)
            {
                offers.Insert(offer);
                return true;
            }
            else
            {
                obj.Amount = offer.Amount;
                obj.isAuto = false;
                obj.LogistAgreed = true;
                result = true;
                offers.Save();
            }

            return result;

        }

        public bool AgreedSellerOffer(LogisticOffer offer)
        {
            bool result = false;
            var offers = _offers.Where(o => o.OrderId == offer.OrderId); ;
            if (offers == null || offers.Count() == 0)
            {

                return false;

            }

            var obj = offers.FirstOrDefault(o => o.LogisticCompanyId == offer.LogisticCompanyId);
            if (obj != null)
            {
                obj.SellerAgreed = true;
                offers.Save();
                result = true;
            }
            return result;

        }

        public bool RemoveOfferDeliveryLogist(LogisticOffer offer)
        {
            var obj = _offers.FirstOrDefault(o => o.LogisticCompanyId == offer.LogisticCompanyId && o.OrderId == offer.OrderId);
            if (obj != null)
            {
                _offers.Delete(obj);
                return true;
            }

            return false;

        }

        public bool RemoveOfferDeliveryOrder(long orderId)
        {
            var objs = _offers.Where(o => o.OrderId == orderId);
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    _offers.Delete(obj);
                }
                return true;
            }

            return false;

        }
        public async Task<LogisticOffer?> GetOfferAsync(string Offerid)
        {
            var result = await _offers.FirstOrDefaultAsync(o => o.OfferId == Offerid);
            return result;
        }

        public async Task<LogisticOffer?> GetOfferAsync(long orderId, long logistId)
        {
            var result = await _offers.FirstOrDefaultAsync(o => o.OrderId == orderId && o.LogisticCompany.Id == logistId);
            return result;
        }
    }
}
