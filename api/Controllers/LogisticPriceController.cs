using api.Hubs;
using api.Models.LogisticPrice;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation;
using Services.Implementation.Exceptions;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("default")]
    public class LogisticPriceController : ControllerBase
    {
        private readonly ILogisticPriceService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<LogisticPriceController> _logger;
        private readonly IGeoService _geoService;
        private readonly IOffersService _offersService;
        private readonly IContragentService _contragentService;
        private readonly IOrderService _orderService;
        private readonly IHubContext<EventsExchange> _hubContext;

        public LogisticPriceController(
            ILogisticPriceService service,
            IMapper mapper,
            ILogger<LogisticPriceController> logger,
            IGeoService geoService,
            IOffersService offersService,
            IContragentService contragentService,
            IOrderService orderService,
            IHubContext<EventsExchange> hubContext
            )
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _geoService = geoService;
            _offersService = offersService;
            _contragentService = contragentService;
            _orderService = orderService;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (id <= 0) { throw new BadRequestException("Неверный идентификатор цены"); }
            var item = await _service.GetBookByIdAsync(id);
            if (item == null) { throw new NotFoundException("Цена", id); }
            return Ok(_mapper.Map<LogisticPriceDto, LogisticPriceModel>(item));
        }

        [HttpGet("history-price/{id}")]
        public async Task<IActionResult> GetHistory(int id)
        {
            if (id <= 0) { throw new BadRequestException("Неверный идентификатор логиста"); }
            var items = await _service.GetHistoryPriceLogist(id);
            if (items == null) { throw new NotFoundException("Цена", id); }
            return Ok(_mapper.Map<List<LogisticPriceDto>, List<LogisticPriceModel>>(items));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(LogisticPriceModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("Нет объекта для создания");
            }

            var dto = _mapper.Map<LogisticPriceModel, LogisticPriceDto>(model);
            dto.LogisticCompany = null;
            dto.Id = 0;
            var createdDto = await _service.CreateAsync(dto);
            //await _offersService.SetAutoCost(_orderService, _contragentService, _service, null, dto.Id);
            await _orderService.SetAutoCost(currentLogistId: createdDto.LogisticCompanyId);
            string message = "";
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "UpdateListOrder", message);
            var createdModel = _mapper.Map<LogisticPriceDto, LogisticPriceModel>(createdDto);
            return Ok(createdModel);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(LogisticPriceModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("Нет объекта для обновления");
            }

            var dto = _mapper.Map<LogisticPriceModel, LogisticPriceDto>(model);
            var isUpdated = await _service.UpdateAsync(dto);
           //await _offersService.SetAutoCost(_orderService, _contragentService, _service, null, dto.Id);
            await _orderService.SetAutoCost(currentLogistId: dto.Id);
            string message = "";
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "UpdateListOrder", message);
            return Ok(isUpdated);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(long? id)
        {
            if (id == null || id == 0)
            {
                throw new BadRequestException("Нулевой идентификатор цены");
            }

            await _service.DeleteAsync(id.Value);
            //await _offersService.SetAutoCost(_orderService, _contragentService, _service);
            await _orderService.SetAutoCost();
            string message = "";
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "UpdateListOrder", message);

            return Ok();
        }
    }
}
