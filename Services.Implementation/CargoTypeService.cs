using ApplicationCore.Books;
using ApplicationCore.Interfaces;
using AutoMapper;
using Services.Abstractions;
using Services.Contracts;

namespace Services.Implementation
{
    public class CargoTypeService : ICargoTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public CargoTypeService(
            IMapper mapper,
            IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<CargoTypeDto?> GetBookByIdAsync(long? id)
        {
            var item = _uow.CargoTypes.GetItemById(id);
            if (item == null) return null;
            return _mapper.Map<CargoType, CargoTypeDto>(item);
        }

        public async Task<CargoTypeDto> CreateAsync(CargoTypeDto dto)
        {
            var cargoType = _mapper.Map<CargoTypeDto, CargoType>(dto);

            var newContragent = await _uow.CargoTypes.AddAsync(cargoType);
            await _uow.CargoTypes.SaveAsync();

            return _mapper.Map<CargoType, CargoTypeDto>(newContragent);
        }

        public async Task<bool> UpdateAsync(CargoTypeDto dto)
        {
            var cargoType = _mapper.Map<CargoTypeDto, CargoType>(dto);

            _uow.CargoTypes.Update(cargoType);
            await _uow.CargoTypes.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            _uow.CargoTypes.Delete(id);
            await _uow.CargoTypes.SaveAsync();

            return true;
        }

        public async Task<List<CargoTypeDto>> GetAllAsync(string? search)
        {
            var items = (await _uow.CargoTypes.GetItemsListAsync(true)).Where(r => search == null || search == string.Empty || r.Name.ToLower().Contains(search.ToLower())).ToList();
            var dtoList = _mapper.Map<List<CargoType>, List<CargoTypeDto>>(items);
            return dtoList;
        }
    }
}
