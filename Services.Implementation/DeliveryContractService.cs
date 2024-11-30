using ApplicationCore.Documents;
using ApplicationCore.Interfaces;
using ApplicationUsers;
using AutoMapper;
using MassTransit;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation.Exceptions;

namespace Services.Implementation
{
    public class DeliveryContractService : IDeliveryContractService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeliveryContractService(
            IMapper mapper,
            IUnitOfWork uow,
            IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _uow = uow;
            _publishEndpoint = publishEndpoint;
        }

        public bool AccessDocumentForEdit(OrderDto order, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public bool AccessDocumentForView(OrderDto order, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public DeliveryContractDto Add(DeliveryContractDto item)
        {
            throw new NotImplementedException();
        }

        public async Task<DeliveryContractDto> AddAsync(DeliveryContractDto item)
        {
            if (item == null) throw new BadRequestException("delivery contract is null");
            var deliveryContract = _mapper.Map<DeliveryContractDto, DeliveryContract>(item);
            deliveryContract.Order = null;
            var result = await _uow.DeliveryContracts.AddAsync(deliveryContract);
            return _mapper.Map<DeliveryContract, DeliveryContractDto>(result);
        }

        public void AddRange(List<DeliveryContractDto> item)
        {
            throw new NotImplementedException();
        }

        public async Task AddRangeAsync(ICollection<DeliveryContractDto> items)
        {
            throw new NotImplementedException();
        }

        public async Task<DeliveryContractDto> CreateDocument()
        {
            DeliveryContractDto? deliveryContractDto = new()
            {
                Id = 0,
                Date = DateTime.Now,
                Status = "Новый"
            };

            return deliveryContractDto;
        }

        public bool Delete(long id)
        {
            var res = _uow.DeliveryContracts.Delete(id);
            return res;
        }

        public DeliveryContractDto? GetDocumentById(long? id)
        {
            throw new NotImplementedException();
        }

        public async Task<DeliveryContractDto?> GetDocumentByIdAsync(long? id)
        {
            var item = await _uow.DeliveryContracts.GetItemByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<DeliveryContract, DeliveryContractDto>(item);
        }

        public async Task<ICollection<DeliveryContractDto>> GetPagedAsync(DeliveryContractFilterDto filterDto)
        {
            var items = await _uow.DeliveryContracts.GetPagedAsync(filterDto);
            return _mapper.Map<List<DeliveryContract>, List<DeliveryContractDto>>(items);
        }

        public IEnumerable<DeliveryContractDto> GetDocumentsList()
        {
            throw new NotImplementedException();
        }

        public async Task<List<DeliveryContractDto>> GetDocumentsListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<DeliveryContractDto> OpenDocument(long? id)
        {
            if (id == null) throw new BadRequestException("не задан идентификатор контракта");

            var userInfo = new UserInfo();
            DeliveryContractDto? deliveryContractDto = new();
            if (id == 0)
            {
                if (!AccessDocumentForView(deliveryContractDto, userInfo))
                    throw new AccessDeniedException("нет доступа к созданию контракта");

                deliveryContractDto = new()
                {
                    Id = 0,
                    Date = DateTime.Now,
                    Status = "Новый"
                };
            }
            else
            {
                var deliveryContract = await _uow.DeliveryContracts.GetItemByIdAsync(id.Value) ??
                    throw new PValidationException($"Не найден контракт с id = {id.Value}", "id");
                deliveryContractDto = _mapper.Map<DeliveryContract, DeliveryContractDto>(deliveryContract);

                if (!AccessDocumentForView(deliveryContractDto, userInfo))
                {
                    throw new AccessDeniedException("нет доступа к документу");  // нет доступа
                }
            }

            if (deliveryContractDto == null) { return new DeliveryContractDto(); }
            return deliveryContractDto;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await _uow.SaveChangesAsync();
        }

        public async Task<DeliveryContractDto> SaveDocument(DeliveryContractDto dto)
        {
            if (dto == null) { return dto; }

            var deliveryContract = _mapper.Map<DeliveryContractDto, DeliveryContract>(dto!);

            deliveryContract.Date= deliveryContract.Date.Value.ToUniversalTime();
            deliveryContract.DateDelivery = deliveryContract.DateDelivery.Value.ToUniversalTime();
            if (dto!.Id == 0)
            {
                deliveryContract.Date = DateTime.UtcNow;
                await _uow.DeliveryContracts.AddAsync(deliveryContract);
            }
            else
            {
                _uow.DeliveryContracts.Update(deliveryContract);
            }
            await _uow.SaveChangesAsync();
            DeliveryContractDto newDto = _mapper.Map<DeliveryContract, DeliveryContractDto>(deliveryContract!);
            //DeliveryContractCreated created = _mapper.Map<DeliveryContractDto, DeliveryContractCreated>(newDto!);
            await _publishEndpoint.Publish(newDto);
            
            return newDto;
        }

        public async Task<DeliveryContractDto?> GetDocumentByNumber(string number, int year)
        {
            throw new NotImplementedException();
        }

        public DeliveryContractDto? Update(DeliveryContractDto item)
        {
            throw new NotImplementedException();
        }

        public bool AccessDocumentForView(DeliveryContractDto item, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public bool AccessDocumentForEdit(DeliveryContractDto item, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public DeliveryContractDto GetContractForOrder(long OrderId)
        {
            var contract = ( _uow.DeliveryContracts.Find(c => c.OrderId == OrderId)).ToList();

            return _mapper.Map<DeliveryContract, DeliveryContractDto>(contract.FirstOrDefault());
        }
    }
}
