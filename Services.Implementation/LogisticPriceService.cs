using ApplicationCore.Entities.Books;
using ApplicationCore.Interfaces;
using AutoMapper;
using Services.Abstractions;
using Services.Contracts;

namespace Services.Implementation
{
    public class LogisticPriceService : ILogisticPriceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public LogisticPriceService(
            IMapper mapper,
            IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;

        }

        public async Task<LogisticPriceDto?> GetBookByIdAsync(long? id)
        {
            var item = _uow.LogisticPrices.GetItemById(id);
            if (item == null) return null;
            return _mapper.Map<LogisticPrice, LogisticPriceDto>(item);
        }

        public async Task<LogisticPriceDto> GetLastPriceLogist(long logistId)
        {
            var prices = (await _uow.LogisticPrices.FindAsync(l => l.LogisticCompanyId == logistId
                && l.StartDate <= DateTime.UtcNow));
            if (prices.Any())
            {
                return _mapper.Map<LogisticPrice, LogisticPriceDto>(prices.OrderBy(l => l.Id).Last());
            }
            return new LogisticPriceDto();
        }
        public async Task<List<LogisticPriceDto>> GetHistoryPriceLogist(long logistId)
        {
            var prices = (await _uow.LogisticPrices.FindAsync(l => l.LogisticCompanyId == logistId));
            if (prices.Any())
            {
                return _mapper.Map<List<LogisticPrice>, List<LogisticPriceDto>>(prices.OrderBy(l => l.StartDate).ToList());
            }
            return new List<LogisticPriceDto>();
        }


        public async Task<LogisticPriceDto> CreateAsync(LogisticPriceDto dto)
        {
            var logisticPrice = _mapper.Map<LogisticPriceDto, LogisticPrice>(dto);

            var newContragent = await _uow.LogisticPrices.AddAsync(logisticPrice);
            await _uow.LogisticPrices.SaveAsync();
            

            return _mapper.Map<LogisticPrice, LogisticPriceDto>(newContragent);
        }

        public async Task<bool> UpdateAsync(LogisticPriceDto dto)
        {
            var logisticPrice = _mapper.Map<LogisticPriceDto, LogisticPrice>(dto);

            _uow.LogisticPrices.Update(logisticPrice);
            await _uow.LogisticPrices.SaveAsync();
            

            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            _uow.LogisticPrices.Delete(id);
            await _uow.LogisticPrices.SaveAsync();          
            return true;
        }
    }
}
