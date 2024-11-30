using Services.Contracts;

namespace Services.Abstractions
{
    public interface ILogisticPriceService : IBookService<LogisticPriceDto>
    {
        public Task<LogisticPriceDto> CreateAsync(LogisticPriceDto dto);
        public Task<bool> UpdateAsync(LogisticPriceDto dto);
        public Task<bool> DeleteAsync(long id);
        public Task<LogisticPriceDto> GetLastPriceLogist(long logistId);
        public Task<List<LogisticPriceDto>> GetHistoryPriceLogist(long logistId);
    }
}
