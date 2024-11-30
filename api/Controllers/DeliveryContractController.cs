using api.Models.DeliveryContract;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation.Exceptions;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("default")]
    public class DeliveryContractController : ControllerBase
    {
        private readonly IDeliveryContractService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<DeliveryContractController> _logger;
        private readonly IGeoService _geoService;

        public DeliveryContractController(
            IDeliveryContractService service,
            IMapper mapper,
            ILogger<DeliveryContractController> logger,
            IGeoService geoService)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _geoService = geoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (id <= 0) { throw new BadRequestException("Неверный идентификатор контракта"); }
            var item = await _service.GetDocumentByIdAsync(id);
            if (item == null) { throw new NotFoundException("Контракт", id); }
            return Ok(_mapper.Map<DeliveryContractDto, DeliveryContractModel>(item));
        }

        [HttpGet("list-contracts")]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] DeliveryContractFilterModel? filterModel)
        {

            var filterDto = _mapper.Map<DeliveryContractFilterModel, DeliveryContractFilterDto>(filterModel);
            var results = await _service.GetPagedAsync(filterDto);
            return Ok(_mapper.Map<List<DeliveryContractDto>, List<DeliveryContractModel>>(results.ToList()));

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync()
        {
            var dto = await _service.CreateDocument();

            var model = _mapper.Map<DeliveryContractDto, DeliveryContractModel>(dto);

            return Ok(model);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditAsync(long id)
        {
            var dto = await _service.OpenDocument(id);
            if (dto == null) { throw new NotFoundException("Контракт", id); }

            var model = _mapper.Map<DeliveryContractDto, DeliveryContractModel>(dto);

            return Ok(model);
        }


        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync(DeliveryContractModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("Нет объекта для сохранения");
            }

            var dto = _mapper.Map<DeliveryContractModel, DeliveryContractDto>(model);

            await _service.SaveDocument(dto);

            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(long? id)
        {

            if (id == null || id == 0)
            {
                throw new BadRequestException("Нулевой идентификатор контракта");
            }

            _service.Delete(id.Value);
            await _service.SaveAsync();

            return Ok();
        }
    }
}
