using api.Models;
using api.Models.Orders;
using ApplicationUsers;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration.UserSecrets;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation;
using Services.Implementation.Exceptions;
using Microsoft.AspNetCore.SignalR;
using api.Hubs;
using StackExchange.Redis;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("GrantCors")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly IContragentService _contragentService;
        private readonly IOffersService _offersService;
        private readonly ILogisticPriceService _logisticPriceService;
        private readonly IDeliveryContractService _deliveryContractService;
        private readonly IIdentityService _identityService;
        private readonly IHubContext<EventsExchange> _hubContext;
        public OrderController(IOrderService service,
            IContragentService contragentService,
            IMapper mapper,
            ILogger<OrderController> logger,
            IOffersService offersService,
            ILogisticPriceService logisticPriceService,
            IDeliveryContractService deliveryContractService,
            IIdentityService identityService,
            IHubContext<EventsExchange> hubContext
            )
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _contragentService = contragentService;
            _offersService = offersService;
            _logisticPriceService = logisticPriceService;
            _deliveryContractService = deliveryContractService;
            _identityService = identityService;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (id <= 0) { throw new BadRequestException("Неверный идентификатор заказа"); }
            var item = await _service.GetDocumentByIdAsync(id);
            if (item == null) { throw new NotFoundException("Заказ", id); }
            return Ok(_mapper.Map<OrderDto, OrderModel>(item));
        }
        [HttpGet]
        public async Task<IActionResult> GetByNumberAsync(string number, int year)
        {
            if (number == String.Empty) { throw new BadRequestException("Неверный номер"); }
            if (year <= 2000) { throw new BadRequestException("Неверный год"); }
            var item = await _service.GetDocumentByNumber(number, year);
            if (item == null) { throw new NotFoundException("Заказ", number); }
            return Ok(_mapper.Map<OrderDto, OrderModel>(item));
        }



        [HttpPost("list-orders")]

        public async Task<IActionResult> GetListAsync(OrderFilterModel? filterModel)
        {

            var filterDto = _mapper.Map<OrderFilterModel, OrderFilterDto>(filterModel);
           // var results = await _service.GetPagedAsync(filterDto);
            var results = await _service.GetPagedListForUserAsync(null, filterDto);

            var model = _mapper.Map<List<OrderDto>, List<OrderModel>>(results.ToList());
            model.ToList().ForEach(r =>
            {
                r.TotalWeight = r.Invoices == null ? 0 : r.Invoices.Sum(i => i.Weight);
            });
            return Ok(model);

        }
        [HttpPost("list-orders-dashboard")]

        public async Task<IActionResult> GetListDashboardAsync(OrderFilterModel? filterModel)
        {

            var filterDto = _mapper.Map<OrderFilterModel, OrderFilterDto>(filterModel);
           // var results = await _service.GetPagedAsync(filterDto);
            var results = await _service.GetPagedListDashboardForUserAsync(filterDto);

            var model = _mapper.Map<List<OrderDto>, List<OrderModel>>(results.ToList());
            model.ToList().ForEach((r) =>
            {
                var dto = results.FirstOrDefault(dto=> dto.Id == r.Id);
                r.TotalWeight = r.Invoices == null ? 0 : r.Invoices.Sum(i => i.Weight);
                r.LogisticOffers = ( _offersService.GetBestOffersAsync(dto).Result)?.ToList();
                r.DeliveryContract =  _deliveryContractService.GetContractForOrder(r.Id);
            });
            return Ok(model);

        }

        [HttpPost("make-offer-tologistic")]

        public async Task<IActionResult> MakeOfferToLogistic( long orderId, long logistic)
        {
            var orderDto = await _service.GetDocumentByIdAsync(orderId);
            if (orderDto == null) { throw new NotFoundException("orderId", orderId); }
            if (logistic == 0) { throw new BadRequestException("ContragentID==0"); }
            var startStatus =orderDto.Status;
            var orderDto_res = await _service.MakeOfferToLogisticCompany(orderDto, logistic);
            await SendMessage(startStatus, orderDto_res);
            var orderModel_res = _mapper.Map<OrderDto, OrderModel>(orderDto_res);

            return Ok(orderModel_res);

        }
        [HttpPost("cancel-offer-tologistic")]

        public async Task<IActionResult> CancelOfferToLogistic( long orderId)
        {
            var orderDto = await _service.GetDocumentByIdAsync(orderId);
            if (orderDto == null) { throw new NotFoundException("orderId", orderId); }
            var startStatus= orderDto.Status;
            var orderDto_res = await _service.CancelOfferToLogisticCompany(orderDto);
            await SendMessage(startStatus, orderDto_res);
            var orderModel_res = _mapper.Map<OrderDto, OrderModel>(orderDto_res);

            return Ok(orderModel_res);

        }


        [HttpPost("refuse-order")]
        public async Task<IActionResult> RefuseOrder(long orderId, long logistic)
        {
            ContragentDto? contragentDto = await _contragentService.GetBookByIdAsync(logistic);
            if (contragentDto == null) { throw new NotFoundException("ContragentId", logistic); }
            OrderDto orderDto = await _service.GetDocumentByIdAsync(orderId);
            if(orderDto == null) { return NotFound(); }
            var startStatus = orderDto.Status;
            var orderDto_res = await _service.RefuseOrder(orderDto, logistic);
            await SendMessage(startStatus, orderDto_res);
            var orderModel_res = _mapper.Map<OrderDto, OrderModel>(orderDto_res);

            return Ok(orderModel_res);

        }


        [HttpPost("set-status-order")]
        public async Task<IActionResult> SetStatusOrder(long orderId, string status)
        {

            UserInfo userInfo = await _identityService.GetUserInfo();
            OrderDto orderDto = await _service.GetDocumentByIdAsync(orderId);
            if (orderDto == null) { return NotFound(); }
            var startStatus = orderDto.Status;
            var orderDto_res = await _service.SetStatusToOrder(userInfo, orderDto, status);
            await SendMessage(startStatus, orderDto_res);
            var orderModel_res = _mapper.Map<OrderDto, OrderModel>(orderDto_res);

            return Ok(orderModel_res);

        }

        [HttpPost("set-new-cost")]
        public async Task<IActionResult> SetNewCostOrderDelivery(decimal newCost, long orderId, long logistId)
        {

            UserInfo userInfo = await _identityService.GetUserInfo();
            if (!userInfo.Logist || userInfo.Settings?.CompanyId != logistId) { throw new AccessDeniedException("Установка стоимости доставки"); }
            await _offersService.SetNewCostDelivery(newCost, orderId, logistId);
            var orderDto = await _service.GetDocumentByIdAsync(orderId);
            var message = $"Предложена новая цена для  {orderDto.Title}";
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Customer",orderDto.SellerId, message);
            return Ok();

        }

        [HttpPost("take-order-to-work")]
        public async Task<IActionResult> TakeOrderToWork(long orderId, long logistic)
        {
            var orderDto = await _service.GetDocumentByIdAsync(orderId);
            if (orderDto == null) { throw new BadRequestException($"orderId={orderId}"); }
            var findedOffer = await _offersService.GetOfferAsync(orderId,logistic);
            if (findedOffer == null) {return NotFound();}
            var startStatus = orderDto.Status;
            await _service.TakeOrderToWork(orderDto, logistic, findedOffer);
            //_offersService.RemoveOfferDeliveryLogist(findedOffer);
            await SendMessage(startStatus, orderDto);
            var orderModel_res = _mapper.Map<OrderDto, OrderModel>(orderDto);
            return Ok(orderModel_res);

        }
        [HttpGet("CreateOrder")]
        public async Task<IActionResult> CreateOrder()
        {
            var dto = await _service.CreateDocument();

            var model = _mapper.Map<OrderDto, OrderModel>(dto);

            return Ok(model);
        }


        [HttpGet("EditOrder")]
        public async Task<IActionResult> EditOrder(long id)
        {
            var dto = await _service.OpenDocument(id);
            if (dto == null) { throw new NotFoundException("Заказ", id); }

            var model = _mapper.Map<OrderDto, OrderModel>(dto);
            model.TotalWeight = model.Invoices == null ? 0 : model.Invoices.Sum(i => i.Weight);
            model.LogisticOffers = (await _offersService.GetBestOffersAsync(dto))?.ToList(); ;
            model.AvailableActions=await _service.GetAvailableActions(dto);
            return Ok(model);

        }


        [Authorize(Roles = "Administrator,Customer")]
        [HttpPost("save-order")]
        public async Task<IActionResult> SaveOrder(OrderModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("нет объекта для сохранения");
            }

            var dto = _mapper.Map<OrderModel, OrderDto>(model);

            var resDto=await _service.SaveDocument(dto);
            await SendMessage(model.Status, dto);
            model = _mapper.Map<OrderDto,OrderModel>(resDto);          
            return Ok(model);


        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteOrder(long? id)
        {

            if (id == null || id == 0)
            {
                throw new BadRequestException("нулевой идентификатор заказа");
            }

            _service.Delete(id.Value);
            await _service.SaveAsync();
            _offersService.RemoveOfferDeliveryOrder(id.Value);
            string message = "";
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "UpdateListOrder", message);
            return Ok();


        }

        [HttpPost("make-offer-to-seller")]
        public async Task<IActionResult> MakeOfferToSeller(LogisticOfferModel logisticOffer)
        {
            await _offersService.SetAutoCost(_service,
              _contragentService,
            _logisticPriceService);
            
            LogisticOffer data = new LogisticOffer()
            {
               
                Amount = logisticOffer.Amount,
                isAuto = logisticOffer.isAuto,
                OrderId = logisticOffer.OrderDtoId,
                LogisticCompanyId = logisticOffer.LogisticCompanyId
            };
            _offersService.MakeOfferToSeller(data,_service);

            _offersService.AgreedSellerOffer(data);
            _offersService.RemoveOfferDeliveryLogist(data);

            return Ok();


        }

        private async Task SendMessage(string startStatus,OrderDto orderDto)
        {

            //string message = "";
            if (startStatus=="Черновик" &&  orderDto.Status == "Новый")
            {
                string message =  "Добавлен новый "  + orderDto.Title;
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist",0, message);
                return;
            }
            if (startStatus == "Новый" && orderDto.Status == "Новый")
            {
                string message = "Обновлен " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist",0, message);
                //await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Новый" && orderDto.Status == "Черновик")
            {
                string message = "Отозван " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist", 0, message);
                //await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Новый" && orderDto.Status == "Предложен")
            {
                string message = "Вам предложен " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist", orderDto.LogisticCompanyId, message);
               // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Предложен" && orderDto.Status == "Новый")
            {
                string message = "Предложение отозвано продавцом или отклонено лог. компанией: " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "",0, message);
               // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Предложен" && orderDto.Status == "В работе")
            {
                string message = "Заказ взят на доставку: " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Customer", orderDto.SellerId, message);
                // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "В работе" && orderDto.Status == "Отгружен")
            {
                string message = "Заказ доставлен потребителю: " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Customer", orderDto.SellerId, message);
                // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Отгружен" && orderDto.Status == "Претензия")
            {
                string message = "Поступила претензия по доставке : " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist", orderDto.LogisticCompanyId, message);
                // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Претензия" && orderDto.Status == "Завершен")
            {
                string message = "Претензия по доставке снята : " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist", orderDto.LogisticCompanyId, message);
                // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }
            if (startStatus == "Отгружен" && orderDto.Status == "Завершен")
            {
                string message = "Заказ завершен: " + orderDto.Title;

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Logist", orderDto.LogisticCompanyId, message);
                // await _hubContext.Clients.Group("Logist").SendAsync("ReceiveMessage", "UpdateListOrder", message);
                return;
            }

        }
    }
}
