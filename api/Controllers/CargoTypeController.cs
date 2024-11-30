using api.Models.CargoType;
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
    public class CargoTypeController : ControllerBase
    {
        private readonly ICargoTypeService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CargoTypeController> _logger;
        private readonly IGeoService _geoService;

        public CargoTypeController(
            ICargoTypeService service,
            IMapper mapper,
            ILogger<CargoTypeController> logger,
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
            if (id <= 0) { throw new BadRequestException("Неверный идентификатор типа груза"); }
            var item = await _service.GetBookByIdAsync(id);
            if (item == null) { throw new NotFoundException("Тип груза", id); }
            return Ok(_mapper.Map<CargoTypeDto, CargoTypeModel>(item));
        }

        [HttpGet("list-cargotypes")]
        public async Task<IActionResult> GetAllAsync(string? search)
        {
            
            var items = await _service.GetAllAsync(search);
            if (items == null) { throw new NotFoundException("Тип груза", "список"); }
            return Ok(_mapper.Map<List<CargoTypeDto>, List<CargoTypeModel>>(items));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(CargoTypeModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("Нет объекта для создания");
            }

            var dto = _mapper.Map<CargoTypeModel, CargoTypeDto>(model);
            var createdDto = await _service.CreateAsync(dto);

            var createdModel = _mapper.Map<CargoTypeDto, CargoTypeModel>(createdDto);
            return Ok(createdModel);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(CargoTypeModel? model)
        {
            if (model == null)
            {
                throw new BadRequestException("Нет объекта для обновления");
            }

            var dto = _mapper.Map<CargoTypeModel, CargoTypeDto>(model);
            var isUpdated = await _service.UpdateAsync(dto);

            return Ok(isUpdated);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(long? id)
        {
            if (id == null || id == 0)
            {
                throw new BadRequestException("Нулевой идентификатор типа груза");
            }

            await _service.DeleteAsync(id.Value);

            return Ok();
        }
    }
}
