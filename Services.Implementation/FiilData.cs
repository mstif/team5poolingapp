using Services.Abstractions;
using Services.Contracts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using System.Text.Json;
using Services.Contracts;
using Redis.OM;

namespace Services.Implementation
{
    public class FillData
    {
        private readonly IOrderService _orderService;
        private readonly IOffersService _offersService;
        private readonly IContragentService _contragentService;
        private readonly ILogisticPriceService _logisticPriceService;
        private readonly RedisConnectionProvider _provider;
        public FillData(IOrderService orderService, IOffersService offersService,
            IContragentService contragentService,
            ILogisticPriceService logisticPriceService,
            RedisConnectionProvider provider)
        {
            _contragentService = contragentService;
            _orderService = orderService;
            _logisticPriceService = logisticPriceService;
            _offersService = offersService;
            _provider = provider;


        }
        public   async Task FillOffers()
        {

            var info = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x => x.ToString());
            if (info.All(x => x != "logisticoffer-idx"))
            {
                await _provider.Connection.CreateIndexAsync(typeof(LogisticOffer));
            }
            if (info.All(x => x != "contragentdto-idx"))
            {
                await _provider.Connection.CreateIndexAsync(typeof(ContragentDto));
            }
            await _offersService.ClearData();
            await _offersService.SetAutoCost(_orderService, _contragentService, _logisticPriceService);

        }
    }
}
