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

namespace Services.Implementation
{
    public class OffersRedisService 
    {
        private readonly RedisCollection<LogisticOfferSet> _offers;
        private readonly RedisConnectionProvider _provider;
        public OffersRedisService(RedisConnectionProvider provider)
        {
            _provider = provider;
            _offers = (RedisCollection<LogisticOfferSet>)provider.RedisCollection<LogisticOfferSet>();
        }



        public ICollection<LogisticOffer>? GetBestOffersAsync(OrderDto order)
        {
            var offers = _offers.FindById(order.Id.ToString());
            return offers?.LogistOffers?.OrderBy(o => o.Amount).ToList();
        }

        public async Task SetAutoCost(IOrderService orderService,
            IContragentService contragentService,
            ILogisticPriceService logisticPriceService)
        {
           
            var allLogists = (await contragentService.GetPagedAsync(new ContragentFilterDto() { NotActiveFilter = true }))
                .Where(c => c.LogisticCompany).ToList();
            
            var allOrders = (await orderService.GetActiveOrderListAsync()).Where(o => o.Status == "Новый");

            foreach (var order in allOrders)
            {
                var offers = await _offers.FindByIdAsync(order.Id.ToString());
                if (offers==null)
                {
                 
                    offers = new LogisticOfferSet()
                    { OrderId = order.Id.ToString(), LogistOffers = new List<LogisticOffer>() };
                    await _offers.InsertAsync(offers);


                }


                foreach (var logist in allLogists)
                {
                    var finded = offers?.LogistOffers?.FirstOrDefault(of => of.LogisticCompany.Id == logist.Id);
                    var price = new LogisticPriceDto() { CostPerTnKm = 1255, CostStart = 4588, LogisticCompanyId = logist.Id };//(await logisticPriceService.GetBookByIdAsync(0));
                    var cost = price?.CostStart ?? 0 + price?.CostPerTnKm ?? 0 * order.TotalDistance * (order.Invoices?.Sum(i => i.Weight) ?? 0);
                    var newoffer = new LogisticOffer { LogisticCompany = logist, Amount = cost, isAuto = true };
                    if (finded == null)
                    {

                        offers?.LogistOffers?.Add(newoffer);

                        foreach (LogisticOffer obj in offers?.LogistOffers??new List<LogisticOffer>())
                        {
                            if (obj.LogisticCompany.Id == logist.Id && obj.isAuto)
                            {
                                obj.Amount = cost;
                                break;
                            }
                        }
                    }
                    else
                    {
                       offers?.LogistOffers?.Add(newoffer);
                    }
                   
                }

                await _offers.UpdateAsync(offers);


            }
        }

        public  bool MakeOfferToSeller(LogisticOffer offer)
        {
            bool result = false;
            var offers =  _offers.FindById(offer.OrderId.ToString());
            if(offers==null || offers.OrderId.ToString() == "0")
            {
                return false;
           
            }
          
            if (offers.LogistOffers.Count()==0)
            {
                offers.LogistOffers.Add(offer);
                _offers.Save();
                return true;

            }
            foreach (LogisticOffer obj in offers.LogistOffers)
            {
                if (obj.LogisticCompany.Id == offer.LogisticCompany.Id)
                {
                    obj.Amount = offer.Amount;
                    obj.isAuto = false;
                    result = true;
                    break;
                }
            }
            //var sett = _offers.FirstOrDefault(o => o.OrderId == offer.Order.Id.ToString()).LogistOffers;
            //foreach(var obj in sett){
            //    if (obj.LogisticCompany.Id == offer.LogisticCompany.Id)
            //    {
            //        obj.Amount = offer.Amount;
            //        obj.isAuto = false;
            //        result = true;
            //        break;
            //    }
            //}
;

            _offers.Save();
            return result;

        }

        public bool MakeOfferToLogist(LogisticOffer offer)
        {
            bool result = false;
            var offers = _offers.FindById(offer.OrderId.ToString());
            if (offers.OrderId.ToString() == "0")
            {
                return false;

            }
            if (offers.LogistOffers.Count() == 0)
            {
                offers.LogistOffers.Add(offer);
                _offers.Save();
                return true;

            }
            foreach (LogisticOffer obj in offers.LogistOffers)
            {
                if (obj.LogisticCompany.Id == offer.LogisticCompany.Id)
                {
                    obj.Amount = offer.Amount;
                    obj.isAuto = false;
                    result = true;
                    break;
                }
            }
            _offers.Save();
            return result;

        }

        public bool RemoveOfferDeliveryLogist(LogisticOffer offer)
        {
            LogisticOffer? offerRemove = null;
            var offers = _offers.FindById(offer.OrderId.ToString());
            foreach (LogisticOffer obj in offers?.LogistOffers??new List<LogisticOffer>())
            {

                if (obj.LogisticCompany.Id == offer.LogisticCompany.Id)
                {
                    offerRemove = obj;
                    break;
                }

            }
            if (offerRemove != null)
            {
                offers?.LogistOffers?.Remove(offerRemove);
                _offers.Save();
                return true;
            }
            return false;

        }

    }
}
