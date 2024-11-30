using ApplicationCore.Books;
using ApplicationCore.Interfaces;
using AutoMapper;
using Redis.OM.Searching;
using Redis.OM;
using Services.Abstractions;
using Services.Contracts;

namespace Services.Implementation
{
    public class ContragentService : IContragentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly RedisCollection<ContragentDto> _cacheContragent;
        private readonly RedisConnectionProvider _provider;

        public ContragentService(
            IMapper mapper,
            IUnitOfWork uow, RedisConnectionProvider provider)
        {
            _mapper = mapper;
            _uow = uow;
            _provider = provider;
            _cacheContragent = (RedisCollection<ContragentDto>)provider.RedisCollection<ContragentDto>();
        }

        public async Task<ContragentDto?> CreateAsync(ContragentDto dto)
        {
            var contragent = _mapper.Map<ContragentDto, Contragent>(dto);

            var newContragent = await _uow.Contragents.AddAsync(contragent);
            await _uow.Contragents.SaveAsync();
            var cached = _cacheContragent.FindById(dto.Id.ToString());
            if (cached != null)
            {
                await _cacheContragent.InsertAsync(cached);
            }

            return _mapper.Map<Contragent, ContragentDto>(newContragent);
        }

        public async Task<ContragentDto?> GetBookByIdAsync(long? id)
        {
            var cached = _cacheContragent.FindById(id.ToString());
            if (cached != null)
            {
                return cached;
            }
            var item = _uow.Contragents.GetItemById(id);
            if (item == null) return null;
            var result = _mapper.Map<Contragent, ContragentDto>(item);
            await _cacheContragent.InsertAsync(result);
            return result;
        }

        public async Task<ICollection<ContragentDto>> GetPagedAsync(ContragentFilterDto filterDto)
        {
            var items = await _uow.Contragents.GetPagedAsync(filterDto);
            return _mapper.Map<List<Contragent>, List<ContragentDto>>(items);
        }

        public async Task<bool> UpdateAsync(ContragentDto dto)
        {
            var contragent = _mapper.Map<ContragentDto, Contragent>(dto);

            _uow.Contragents.Update(contragent);
            await _uow.Contragents.SaveAsync();
            var cached = _cacheContragent.FindById(dto.Id.ToString());
            if (cached != null)
            {
                await _cacheContragent.UpdateAsync(dto);
            }
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            _uow.Contragents.Delete(id);
            await _uow.Contragents.SaveAsync();
            var cached = _cacheContragent.FindById(id.ToString());
            if (cached != null)
            {
                await _cacheContragent.DeleteAsync(cached);
            }
            return true;
        }
    }
}
