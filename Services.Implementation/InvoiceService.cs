using ApplicationCore.Books;
using ApplicationCore.Documents;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using AutoMapper;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationUsers;
namespace Services.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IDeliveryContractService _deliveryContractService;
        private readonly IOffersService _offersService;
        private readonly IContragentService _contragentService;
        private readonly ILogisticPriceService _logisticPriceService;
        private readonly IOrderService _orderService;
        public InvoiceService(IMapper mapper,
            IUnitOfWork uow,
            IDeliveryContractService deliveryContractService,
            IOffersService offersService,
            IContragentService contragentService,
            ILogisticPriceService logisticPriceService,
            IOrderService orderService)
        {
            _mapper = mapper;
            _uow = uow;
            _deliveryContractService = deliveryContractService;
            _offersService = offersService;
            _contragentService = contragentService;
            _logisticPriceService = logisticPriceService;
            _orderService = orderService;
        }

        public InvoiceDto Add(InvoiceDto itemDto)
        {
            if (itemDto == null) throw new BadRequestException("invoice is null");
            var invoice = _mapper.Map<InvoiceDto, Invoice>(itemDto);
            var result = _uow.Invoices.Add(invoice);
            _uow.SaveChanges();
            return _mapper.Map<Invoice, InvoiceDto>(result);

        }

        public async Task<InvoiceDto> AddAsync(InvoiceDto itemDto)
        {
            if (itemDto == null) throw new BadRequestException("invoice is null");
            var invoice = _mapper.Map<InvoiceDto, Invoice>(itemDto);
            var result = await _uow.Invoices.AddAsync(invoice);
            _uow.SaveChanges();
            return _mapper.Map<Invoice, InvoiceDto>(result);

        }

        public void AddRange(List<InvoiceDto> itemsDto)
        {
            if (itemsDto == null) throw new BadRequestException("invoice is null");
            var invoices = _mapper.Map<List<InvoiceDto>, List<Invoice>>(itemsDto);
            _uow.Invoices.AddRange(invoices);

        }

        public async Task<InvoiceDto> OpenDocument(long? id)
        {
            if (id == null) throw new BadRequestException("не задан идентификатор накладной");

            //var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userInfo = new UserInfo();
            InvoiceDto? invoiceDto = new InvoiceDto();
            if (id == 0)
            {
                if (!AccessDocumentForView(invoiceDto, userInfo))
                    throw new AccessDeniedException("нет доступа к созданию накладной"); //нет доступа к созданию заказа

                invoiceDto = new InvoiceDto
                {
                    Id = 0,
                    Date = DateTime.Now,
                    StatusDoc = InvoiceStatus.New
                };
            }
            else
            {
                var invoice = await _uow.Invoices.GetItemByIdAsync(id.Value) ?? throw new PValidationException($"Не найдена накладная с id = {id.Value}", "id");
                invoiceDto = _mapper.Map<Invoice, InvoiceDto>(invoice);

                if (!AccessDocumentForView(invoiceDto, userInfo))
                {
                    throw new AccessDeniedException("нет доступа к документу ");  //нет доступа
                }

            }

            if (invoiceDto == null) { return new InvoiceDto(); }
            //var cargoTypesTask = await _uow.CargoTypes.GetItemsListAsync(false);           
            //var logisticsCompanies = _uow.Contragents.Find(c => c.LogisticCompany);
            //var userCustomers = SettingsUser.GetFromJson(user?.SettingsUser)?.UserCustomers?.Select(i => i.ContragentID)?.ToList();
            //var userCustomersList = new List<ContragentDto>() { invoiceDto.Seller };
            //if (userCustomers?.Any() ?? false)
            //{
            //    userCustomersList = _mapper.Map<List<Contragent>,List<ContragentDto>>(_uow.Contragents.Find(c => userCustomers.Contains(c.Id)).ToList());
            //}
            //if (userInfo.Administrator)
            //{
            //    userCustomersList = _mapper.Map<List<Contragent>, List<ContragentDto>>(_uow.Contragents.Find(c => c.ClientCompany).ToList());
            //}

            //invoiceDto.CargoTypes = CargoTypeDTO.GetListFromDomain(cargoTypesTask.ToList())!;


            //invoiceDto.LogisticCompanyList = logisticsCompanies.ToList();
            //invoiceDto.UserCustomersList = userCustomersList;
            return invoiceDto;
        }


        public async Task<InvoiceDto> CreateDocument()
        {


            var userInfo = new UserInfo() { Logist = false };
            InvoiceDto? invoiceDto;

            if (userInfo.Logist) throw new AccessDeniedException("Новая ттн"); //нет доступа к созданию заказа
            invoiceDto = new InvoiceDto
            {
                Id = 0,
                Date = DateTime.Now,
                StatusDoc = InvoiceStatus.New
            };


            return invoiceDto;
        }

        public bool AccessDocumentForView(InvoiceDto invoice, UserInfo user)
        {
            return true; //TODO убрать
            if (user == null) { return false; }
            if (user.Administrator) return true;

            if (user.Settings == null) { return false; }
            bool allowed = false;
            var settings = user.Settings;
            if (settings == null) { throw new Exception("Error read settings user!"); }

            if (user.Logist)
            {
                var company = settings.CompanyId;
                if (company == null) return false;

                allowed = invoice.Order.LogisticCompanyId == company || invoice.Order.LogisticCompanyId == 0;
                if (invoice.Id == 0)
                {
                    allowed = false;
                }


            }
            else
            {
                if (user.Customer)
                {
                    var userCustomerId = settings.CompanyId;
                    if (userCustomerId == null || userCustomerId == 0) { return false; }
                    if (userCustomerId==invoice.Order.SellerId.Value) { allowed = true; }
                }
            }


            return allowed;


        }


        public bool AccessDocumentForEdit(InvoiceDto invoice, UserInfo user)
        {
            return true; //TODO убрать
            if (user == null) { return false; }
            if (user.Administrator) return true;

            if (user.Settings == null) { return false; }
            bool allowed = false;
            var settings = user.Settings;
            if (settings == null) { throw new Exception("Error read settings user!"); }

            if (user.Logist)
            {
                allowed = false;
            }
            else
            {
                if (user.Customer)
                {
                    var userCustomerId = settings.CompanyId;
                    if (userCustomerId == null || userCustomerId==0) { return false; }
                    if (userCustomerId == invoice.Order.SellerId.Value) { allowed = true; }
                }
            }


            return allowed;


        }

        public async Task AddRangeAsync(ICollection<InvoiceDto> itemsDto)
        {
            if (itemsDto == null) throw new BadRequestException("invoice is null");
            var invoices = _mapper.Map<List<InvoiceDto>, List<Invoice>>(itemsDto.ToList());
            await _uow.Invoices.AddRangeAsync(invoices);

        }

        public bool Delete(long id)
        {
            var res = _uow.Invoices.Delete(id);
            return res;
        }

        public InvoiceDto? GetDocumentById(long? id)
        {
            var item = _uow.Invoices.GetItemById(id);
            if (item == null) return null;
            return _mapper.Map<Invoice, InvoiceDto>(item);
        }

        public async Task<InvoiceDto?> GetDocumentByIdAsync(long? id)
        {
            var item = await _uow.Invoices.GetItemByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<Invoice, InvoiceDto>(item);
        }

        public async Task<InvoiceDto?> GetDocumentByNumber(string number, int year)
        {
            var item = await _uow.Invoices.FindAsync(o => o.Number == number || o.Date?.Year == year);
            if (item == null) return null;
            return _mapper.Map<Invoice, InvoiceDto>(item.FirstOrDefault() ?? new Invoice());
        }

        public IEnumerable<InvoiceDto> GetDocumentsList()
        {
            return _mapper.Map<List<Invoice>, List<InvoiceDto>>(_uow.Invoices.GetItemsList(false).ToList());
        }

        public async Task<List<InvoiceDto>> GetDocumentsListAsync()
        {
            var items = await _uow.Invoices.GetItemsListAsync(false);
            return _mapper.Map<List<Invoice>, List<InvoiceDto>>(items);
        }



        public List<InvoiceDto> GetInvoices()
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<InvoiceDto>> GetPagedAsync(InvoiceFilterDto filterDto)
        {
            var items = await _uow.Invoices.GetPagedAsync(filterDto);
            return _mapper.Map<List<Invoice>, List<InvoiceDto>>(items);
        }

        public void Save()
        {
            _uow.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _uow.SaveChangesAsync();
        }

        public async Task<InvoiceDto?> SetStatusToInvoice(UserInfo userInfo, InvoiceDto invoiceDto, InvoiceStatus status)
        {
            InvoiceDto? result = null;
            var allowedStatuses = AllowedEditStatuses(userInfo);
            if (true)//allowedStatuses.Contains(status))
            {
                invoiceDto.StatusDoc = status;
                result = Update(invoiceDto);
                await _uow.SaveChangesAsync();
            }
            else throw new AccessDeniedException($"Статус ttn {status}");
            return invoiceDto;
        }

        public static List<InvoiceStatus> AllowedEditStatuses(UserInfo userInfo)
        {
            if (userInfo == null) return new List<InvoiceStatus>();
            if (userInfo.Logist) return new List<InvoiceStatus>() { InvoiceStatus.Shipped, InvoiceStatus.Delivered, InvoiceStatus.Passed};
            if (userInfo.Customer) return new List<InvoiceStatus>() { InvoiceStatus.Shipped, InvoiceStatus.Delivered, InvoiceStatus.Passed, InvoiceStatus.New };
            if (userInfo.Administrator) new List<InvoiceStatus>() { InvoiceStatus.Shipped, InvoiceStatus.Delivered, InvoiceStatus.Passed, InvoiceStatus.New };
            return new List<InvoiceStatus>();
        }
        public async Task<InvoiceDto>  SaveDocument(InvoiceDto invoiceDto)
        {
            if (invoiceDto == null) { return invoiceDto; }
            Contragent? contragent = _uow.Contragents.GetItemById(invoiceDto?.DeliveryPointId) ?? throw new PValidationException($"контрагент с id = {invoiceDto?.DeliveryPointId} не найден", "");
            var order = await _orderService.GetDocumentByIdAsync(invoiceDto?.OrderId);
            if (order == null) { throw new NotFoundException("Заказ", invoiceDto?.OrderId); }
            //var mapper = new MapperConfiguration(cfg => cfg.CreateMap<InvoiceDTO, Invoice>()).CreateMapper();
            var invoice = _mapper.Map<InvoiceDto, Invoice>(invoiceDto!);

            if (invoiceDto!.Id == 0)
            {
                invoice.Date = DateTime.UtcNow;
                invoice.Number = _uow.Numerators.GetNextStringNumber(ObjectTypes.Invoice, order.SellerId!.Value, order.Date.Value);
                await _uow.Invoices.AddAsync(invoice);
            }
            else
            {

                _uow.Invoices.Update(invoice);
            }
            await _uow.SaveChangesAsync();
            await _orderService.SetAutoCost(currentOrderId: invoiceDto?.OrderId);
            return _mapper.Map<Invoice, InvoiceDto>(invoice!);

        }

        public InvoiceDto? Update(InvoiceDto itemDto)
        {
            if (itemDto == null) return null;
            var res = _uow.Invoices.Update(_mapper.Map<InvoiceDto, Invoice>(itemDto));
            return _mapper.Map<Invoice, InvoiceDto>(res);


        }

        public async Task<ICollection<InvoiceDto>> GetPagedListForUserAsync(UserInfo userInfo, InvoiceFilterDto filterDto)
        {
            var items = await _uow.Invoices.GetPagedAsync(filterDto);
            return (_mapper.Map<List<Invoice>, List<InvoiceDto>>(items)).Where(o => AccessDocumentForView(o, userInfo)).ToList();
        }


    }
}
