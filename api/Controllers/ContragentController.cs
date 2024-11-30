using api.Models.Contragents;
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
    public class ContragentController : ControllerBase
    {
        private readonly IContragentService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ContragentController> _logger;
        private readonly IGeoService _geoService;

        public ContragentController(
            IContragentService service,
            IMapper mapper,
            ILogger<ContragentController> logger,
            IGeoService geoService)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _geoService = geoService;   
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int? id)
        {
            if(id == 0) return Ok(new ContragentModel() );
            if (id < 0 || id==null) { throw new BadRequestException("Неверный идентификатор контрагента"); }
            var item = await _service.GetBookByIdAsync(id);
            if (item == null) { throw new NotFoundException("Контрагент", id); }
           // var res = await _geoService.GetCoordinatesAddress(item?.Address);
            return Ok(_mapper.Map<ContragentDto, ContragentModel>(item));
        }

        [HttpGet("list-contragents")]
        public async Task<IActionResult> GetListAsync([FromQuery] ContragentFilterModel? filterModel)
        {
            var filterDto = _mapper.Map<ContragentFilterModel, ContragentFilterDto>(filterModel);
            var results = await _service.GetPagedAsync(filterDto);
            return Ok(_mapper.Map<List<ContragentDto>, List<ContragentModel>>(results.ToList()));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(ContragentModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("Нет объекта для создания");
            }

            var dto = _mapper.Map<ContragentModel, ContragentDto>(model);
            var createdDto = await _service.CreateAsync(dto);

            var createdModel = _mapper.Map<ContragentDto, ContragentModel>(createdDto);
            return Ok(createdModel);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync([FromBody]ContragentModel? model)
        {
            if (model == null || model.Id == 0)
            {
                throw new BadRequestException("Нет объекта для обновления");
            }

            var dto = _mapper.Map<ContragentModel, ContragentDto>(model);
            var isUpdated = await _service.UpdateAsync(dto);

            return Ok(model);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(long? id)
        {
            if (id == null || id == 0)
            {
                throw new BadRequestException("Нулевой идентификатор контрагента");
            }

            await _service.DeleteAsync(id.Value);

            return Ok();
        }
    }
}
