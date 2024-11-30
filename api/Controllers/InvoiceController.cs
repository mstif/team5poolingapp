using api.Hubs;
using ApplicationCore.Enums;
using ApplicationUsers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation.Exceptions;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {

        private readonly IInvoiceService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceController> _logger;
        private readonly IContragentService _contragentService;
        private readonly IHubContext<EventsExchange> _hubContext;
        public InvoiceController(IInvoiceService service,
            IContragentService contragentService,
            IMapper mapper,
            ILogger<InvoiceController> logger=null,
            IHubContext<EventsExchange> hubContext=null)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _contragentService = contragentService;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (id <= 0) { throw new BadRequestException("Неверный идентификатор ттн"); }
            var item = await _service.GetDocumentByIdAsync(id);
            if (item == null) { throw new NotFoundException("ТТН", id); }
            return Ok(_mapper.Map<InvoiceDto, InvoiceModel>(item));
        }
        [HttpGet]
        public async Task<IActionResult> GetByNumberAsync(string number, int year)
        {
            if (number == String.Empty) { throw new BadRequestException("Неверный номер"); }
            if (year <= 2000) { throw new BadRequestException("Неверный год"); }
            var item = await _service.GetDocumentByNumber(number, year);
            if (item == null) { throw new NotFoundException("Заказ", number); }
            return Ok(_mapper.Map<InvoiceDto, InvoiceModel>(item));
        }



        [HttpPost("list-invoices")]

        public async Task<IActionResult> GetListAsync(InvoiceFilterModel? filterModel)
        {

            var filterDto = _mapper.Map<InvoiceFilterModel, InvoiceFilterDto>(filterModel);
            var results = await _service.GetPagedAsync(filterDto);
            return Ok(_mapper.Map<List<InvoiceDto>, List<InvoiceModel>>(results.ToList()));

        }


        [HttpPost("set-status-invoice")]
        public async Task<IActionResult> SetStatusInvoice(InvoiceModel invoiceModel, InvoiceStatus status)
        {

            UserInfo userInfo = new UserInfo();
            var itemDto = _mapper.Map<InvoiceModel, InvoiceDto>(invoiceModel);
            var itemDto_res = await _service.SetStatusToInvoice(userInfo, itemDto, status);
            var itemModel_res = _mapper.Map<InvoiceDto, InvoiceModel>(itemDto_res);

            return Ok(itemModel_res);

        }
        
        [HttpGet("create-invoice")]
        public async Task<IActionResult> CreateInvoice()
        {
            var dto = await _service.CreateDocument();

            var model = _mapper.Map<InvoiceDto, InvoiceModel>(dto);

            return Ok(model);
        }

        [HttpGet("edit-invoice")]
        public async Task<IActionResult> EditInvoice(long id)
        {
            var dto = await _service.OpenDocument(id);
            if (dto == null) { throw new NotFoundException("ТТН", id); }

            var model = _mapper.Map<InvoiceDto, InvoiceModel>(dto);

            return Ok(model);


        }


        [HttpPost("save-invoice")]
        public async Task<IActionResult> SaveOrder(InvoiceModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("нет объекта для сохранения");
            }

            var dto = _mapper.Map<InvoiceModel, InvoiceDto>(model);
            dto.DeliveryPointId= model.DeliveryPoint?.Id;
            dto = await _service.SaveDocument(dto);
            string message = "Обновлен " + dto.Order?.Title;
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "UpdateListOrder", message);

            return Ok(_mapper.Map<InvoiceDto, InvoiceModel>(dto));


        }

        [HttpPost("delete-invoice")]
        public async Task<IActionResult> DeleteOrder(long? id)
        {

            if (id == null || id == 0)
            {
                throw new BadRequestException("нулевой идентификатор ттн");
            }

            _service.Delete(id.Value);
            await _service.SaveAsync();


            return Ok();
        }
    }
}
