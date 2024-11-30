using ApplicationCore.Documents;
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
    public class OffersService 
    {


        public ConcurrentDictionary<long, List<LogisticOffer>> LogisticOffersList
        { get; set; } = new ConcurrentDictionary<long, List<LogisticOffer>>();

        public ICollection<LogisticOffer> GetBestOffersAsync(OrderDto order)
        {
            var offers = LogisticOffersList[order.Id];
            return offers.OrderBy(o => o.Amount).ToList();
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
                if (!LogisticOffersList.ContainsKey(order.Id))
                {
                    LogisticOffersList.TryAdd(order.Id, new List<LogisticOffer>());
                }


                var offers = LogisticOffersList[order.Id];

                foreach (var logist in allLogists)
                {
                    var finded = offers.FirstOrDefault(of => of.LogisticCompany.Id == logist.Id);
                    var price = (await logisticPriceService.GetBookByIdAsync(0));
                    var cost = price?.CostStart ?? 0 + price?.CostPerTnKm ?? 0 * order.TotalDistance * (order.Invoices?.Sum(i => i.Weight) ?? 0);
                    var newoffer = new LogisticOffer { LogisticCompany = logist, Amount = cost, isAuto = true };
                    if (finded == null)
                    {

                        LogisticOffersList[order.Id].Add(newoffer);
                        foreach (LogisticOffer obj in LogisticOffersList[order.Id])
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
                        LogisticOffersList[order.Id].Add(newoffer);
                    }
                }


            }
        }

        public bool MakeOfferToSeller(LogisticOffer offer)
        {
            bool result = false;
            foreach (LogisticOffer obj in LogisticOffersList[offer.OrderId])
            {
                if (obj.LogisticCompany.Id == offer.LogisticCompany.Id)
                {
                    obj.Amount = offer.Amount;
                    obj.isAuto = false;
                    result = true;
                    break;
                }
            }
            return result;

        }
        public bool RemoveOfferDeliveryLogist(LogisticOffer offer)
        {
            LogisticOffer? offerRemove = null;
            foreach (LogisticOffer obj in LogisticOffersList[offer.OrderId])
            {

                if (obj.LogisticCompany.Id == offer.LogisticCompany.Id)
                {
                    offerRemove = obj;
                    break;
                }

            }
            if (offerRemove != null)
            {
                LogisticOffersList[offer.OrderId].Remove(offerRemove);
                return true;
            }
            return false;

        }

    }
}
