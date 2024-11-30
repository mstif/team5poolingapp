using Services.Contracts;

namespace Services.Abstractions
{
    public interface IContragentService : IBookService<ContragentDto>
    {
        public Task<ICollection<ContragentDto>> GetPagedAsync(ContragentFilterDto filterDto);
        public Task<ContragentDto> CreateAsync(ContragentDto dto);
        public Task<bool> UpdateAsync(ContragentDto dto);
        public Task<bool> DeleteAsync(long id);
    }
}
