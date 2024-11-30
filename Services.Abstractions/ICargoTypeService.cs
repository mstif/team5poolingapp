using Services.Contracts;

namespace Services.Abstractions
{
    public interface ICargoTypeService : IBookService<CargoTypeDto>
    {
        public Task<CargoTypeDto> CreateAsync(CargoTypeDto dto);
        public Task<bool> UpdateAsync(CargoTypeDto dto);
        public Task<bool> DeleteAsync(long id);
        public Task<List<CargoTypeDto>> GetAllAsync(string search);
    }
}
