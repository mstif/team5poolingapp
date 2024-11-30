using ApplicationCore.Books;
using ApplicationCore.Documents;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Documents;
using ApplicationUsers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections;
using ApplicationCore.Enums;
using Services.Implementation.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;





namespace Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IDeliveryContractService _deliveryContractService;
        private readonly IOffersService _offersService;
        private readonly IContragentService _contragentService;
        private readonly ILogisticPriceService _logisticPriceService;
        private readonly IIdentityService _identityService;
        private readonly IGeoService _geoService;
        public OrderService(IMapper mapper,
            IUnitOfWork uow,
            IDeliveryContractService deliveryContractService,
            IOffersService offersService,
            IContragentService contragentService,
            ILogisticPriceService logisticPriceService,
            IIdentityService identityService,
            IGeoService geoService)
        {
            _mapper = mapper;
            _uow = uow;
            _deliveryContractService = deliveryContractService;
            _offersService = offersService;
            _contragentService = contragentService;
            _logisticPriceService = logisticPriceService;
            _identityService = identityService;
            _geoService = geoService;
        }

        public OrderDto Add(OrderDto itemDto)
        {
            if (itemDto == null) throw new BadRequestException("order is null"); //TODO добавить нормальное логирование
            var order = _mapper.Map<OrderDto, Order>(itemDto);
            var result = _uow.Orders.Add(order);
            _uow.SaveChanges();
            //_offersService.SetAutoCost(this, _contragentService, _logisticPriceService, order.Id);
             SetAutoCost(currentOrderId: order.Id).Wait();
            return _mapper.Map<Order, OrderDto>(result);

        }

        public async Task<OrderDto> AddAsync(OrderDto itemDto)
        {
            if (itemDto == null) throw new BadRequestException("order is null");
            var order = _mapper.Map<OrderDto, Order>(itemDto);
            var result = await _uow.Orders.AddAsync(order);
            return _mapper.Map<Order, OrderDto>(result);
        }

        public void AddRange(List<OrderDto> itemsDto)
        {
            if (itemsDto == null) throw new BadRequestException("order is null");
            var orders = _mapper.Map<List<OrderDto>, List<Order>>(itemsDto);
            _uow.Orders.AddRange(orders);

        }

        public async Task<OrderDto> OpenDocument(long? id)
        {
            if (id == null) throw new BadRequestException("не задан идентификатор заказа");

            //var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userInfo = await _identityService.GetUserInfo();
            OrderDto? orderDto = new OrderDto();
            if (id == 0)
            {
                if (!AccessDocumentForView(orderDto, userInfo))
                    throw new AccessDeniedException("нет доступа к созданию заказа"); //нет доступа к созданию заказа

                orderDto = new OrderDto
                {
                    Id = 0,
                    Date = DateTime.Now,
                    Status = "Новый"
                };
            }
            else
            {
                var order = await _uow.Orders.GetItemByIdAsync(id.Value) ?? throw new PValidationException($"Не найден заказ с id = {id.Value}", "id");
                orderDto = _mapper.Map<Order, OrderDto>(order);

                if (!AccessDocumentForView(orderDto, userInfo))
                {
                    throw new AccessDeniedException("нет доступа к документу ");  //нет доступа
                }

            }

            if (orderDto == null) { return new OrderDto(); }
            //var cargoTypesTask = await _uow.CargoTypes.GetItemsListAsync(false);           
            //var logisticsCompanies = _uow.Contragents.Find(c => c.LogisticCompany);
            //var userCustomers = SettingsUser.GetFromJson(user?.SettingsUser)?.UserCustomers?.Select(i => i.ContragentID)?.ToList();
            //var userCustomersList = new List<ContragentDto>() { orderDto.Seller };
            //if (userCustomers?.Any() ?? false)
            //{
            //    userCustomersList = _mapper.Map<List<Contragent>,List<ContragentDto>>(_uow.Contragents.Find(c => userCustomers.Contains(c.Id)).ToList());
            //}
            //if (userInfo.Administrator)
            //{
            //    userCustomersList = _mapper.Map<List<Contragent>, List<ContragentDto>>(_uow.Contragents.Find(c => c.ClientCompany).ToList());
            //}

            //orderDto.CargoTypes = CargoTypeDTO.GetListFromDomain(cargoTypesTask.ToList())!;


            //orderDto.LogisticCompanyList = logisticsCompanies.ToList();
            //orderDto.UserCustomersList = userCustomersList;
            return orderDto;
        }
        public async Task<List<PairValue>> GetAvailableActions(OrderDto order)
        {
            var result = new List<PairValue>();
            var userInfo = await _identityService.GetUserInfo();

            if (order.LogisticCompanyId == 0 || order.LogisticCompanyId == null)
            {
                if (order.Status == "Черновик")
                {
                    if (userInfo.Customer || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "SetNewStatus", Title = "Опубликовать заказ" });
                    }
                }
                if (order.Status == "Новый")
                {
                    if (userInfo.Customer || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "SetDraftStatus", Title = "Отозвать заказ" });
                    }
                }
            }
            if (order.LogisticCompanyId > 0)
            {


                if (order.Status == "Новый")
                {
                    if (userInfo.Customer || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "MakeOfferToLogisticCompany", Title = "Предложить лог. компании" });
                    }
                }
                if (order.Status == "Предложен")
                {
                    if (userInfo.Customer || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "CancelOfferToLogisticCompany", Title = "Отозвать предложение лог. компании" });
                    }
                    if (userInfo.Logist || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "TakeOrderToWork", Title = "Взять заказ в работу" });
                        result.Add(new PairValue() { Id = "RefuseOrder", Title = "Отказаться от заказа" });
                    }
                }
                if (order.Status == "В работе")
                {
                    if (userInfo.Logist || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "FinishOrderBySeller", Title = "Доставлен потребителю" });
                    }

                }
                if (order.Status == "Отгружен")
                {
                    if (userInfo.Customer || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "FinishOrder", Title = "Заказ завершен" });
                        result.Add(new PairValue() { Id = "Claim", Title = "Претензия к доставке" });
                    }

                }
                if (order.Status == "Претензия")
                {
                    if (userInfo.Customer || userInfo.Administrator)
                    {
                        result.Add(new PairValue() { Id = "FinishOrder", Title = "Заказ завершен" });
                    }

                }

            }
            return result;
        }
        public async Task<OrderDto> CreateDocument()
        {


            var userInfo = await _identityService.GetUserInfo();
            OrderDto? orderDto;

            if (userInfo.Logist) throw new AccessDeniedException("Новый заказ"); //нет доступа к созданию заказа
            orderDto = new OrderDto
            {
                Id = 0,
                Date = DateTime.Now,
                Status = "Черновик"
            };




            //var cargoTypesTask = await _uow.CargoTypes.GetItemsListAsync(false);           
            //var logisticsCompanies = _uow.Contragents.Find(c => c.LogisticCompany);
            //var userCustomers = SettingsUser.GetFromJson(user?.SettingsUser)?.UserCustomers?.Select(i => i.ContragentID)?.ToList();
            //var userCustomersList = new List<ContragentDto>() { orderDto.Seller };
            //if (userCustomers?.Any() ?? false)
            //{
            //    userCustomersList = _mapper.Map<List<Contragent>,List<ContragentDto>>(_uow.Contragents.Find(c => userCustomers.Contains(c.Id)).ToList());
            //}
            //if (userInfo.Administrator)
            //{
            //    userCustomersList = _mapper.Map<List<Contragent>, List<ContragentDto>>(_uow.Contragents.Find(c => c.ClientCompany).ToList());
            //}

            //orderDto.CargoTypes = CargoTypeDTO.GetListFromDomain(cargoTypesTask.ToList())!;


            //orderDto.LogisticCompanyList = logisticsCompanies.ToList();
            //orderDto.UserCustomersList = userCustomersList;
            return orderDto;
        }

        public bool AccessDocumentForView(OrderDto order, UserInfo user1)
        {
            //return true; //TODO убрать
            var user = _identityService.GetUserInfo().Result;
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

                allowed = order.LogisticCompanyId == company || order.LogisticCompanyId == 0 || order.LogisticCompanyId == null;
                if (order.Id == 0)
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
                    if (userCustomerId == order.SellerId.Value) { allowed = true; }
                }
            }


            return allowed;


        }


        public bool AccessDocumentForEdit(OrderDto order, UserInfo user1)
        {
            //return true; //TODO убрать
            var user = _identityService.GetUserInfo().Result;
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
                    if (userCustomerId == null || userCustomerId == 0) { return false; }
                    if (userCustomerId == order.SellerId.Value) { allowed = true; }
                }
            }


            return allowed;


        }

        public async Task AddRangeAsync(ICollection<OrderDto> itemsDto)
        {
            if (itemsDto == null) throw new BadRequestException("order is null");
            var orders = _mapper.Map<List<OrderDto>, List<Order>>(itemsDto.ToList());
            await _uow.Orders.AddRangeAsync(orders);

        }

        public bool Delete(long id)
        {
            var res = _uow.Orders.Delete(id);

            return res;
        }

        public OrderDto? GetDocumentById(long? id)
        {
            var item = _uow.Orders.GetItemById(id);
            if (item == null) return null;
            return _mapper.Map<Order, OrderDto>(item);
        }

        public async Task<OrderDto?> GetDocumentByIdAsync(long? id)
        {
            var item = await _uow.Orders.GetItemByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<Order, OrderDto>(item);
        }

        public async Task<OrderDto?> GetDocumentByNumber(string number, int year)
        {
            var item = await _uow.Orders.FindAsync(o => o.Number == number || o.Date?.Year == year);
            if (item == null) return null;
            return _mapper.Map<Order, OrderDto>(item.FirstOrDefault() ?? new Order());
        }

        public IEnumerable<OrderDto> GetDocumentsList()
        {
            return _mapper.Map<List<Order>, List<OrderDto>>(_uow.Orders.GetItemsList(false).ToList());
        }

        public async Task<List<OrderDto>> GetDocumentsListAsync()
        {
            var items = await _uow.Orders.GetItemsListAsync(false);
            return _mapper.Map<List<Order>, List<OrderDto>>(items);
        }

        public async Task<List<OrderDto>> GetActiveOrderListAsync()
        {
            var items = (await _uow.Orders.FindAsync(o => o.Status != "Завершен")).ToList();
            return _mapper.Map<List<Order>, List<OrderDto>>(items);
        }



        public List<InvoiceDto> GetInvoices(OrderDto orderDto)
        {
            var invoices = _uow.Invoices.Find(o=>o.OrderId == orderDto.Id).ToList();

            return _mapper.Map<List<Invoice>, List<InvoiceDto>>(invoices);
        }

        public async Task<ICollection<OrderDto>> GetPagedAsync(OrderFilterDto filterDto)
        {
            var items = await _uow.Orders.GetPagedAsync(filterDto);
            return _mapper.Map<List<Order>, List<OrderDto>>(items);
        }

        public void Save()
        {
            _uow.SaveChanges();

        }

        public async Task SaveAsync()
        {
            await _uow.SaveChangesAsync();
        }

        public async Task<OrderDto> SaveDocument(OrderDto orderDto)
        {
            var userInfo = await _identityService.GetUserInfo();
            if (!AccessDocumentForEdit(orderDto, userInfo))
            {
                throw new AccessDeniedException("Нет доступа к записи документа");
            }
            if (orderDto == null) { return orderDto; }
            Contragent? contragent = _uow.Contragents.GetItemById(orderDto?.SellerId) ?? throw new PValidationException($"контрагент с id = {orderDto?.SellerId} не найден", "");


            CargoType? cargoType = _uow.CargoTypes.GetItemById(orderDto?.CargoTypeId);

            // валидация
            if (cargoType == null)
                throw new PValidationException($"тип груза с id = {orderDto?.CargoTypeId} не найден", "");




            //var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, Order>()).CreateMapper();
            var order = _mapper.Map<OrderDto, Order>(orderDto!);

            order.DateDeparture = order.DateDeparture.Value.ToUniversalTime();

            if (orderDto!.Id == 0)
            {
                order.Date = DateTime.UtcNow;
                order.Number = _uow.Numerators.GetNextStringNumber(ObjectTypes.Order, order.SellerId!.Value, order.Date.Value);
                order.DateCreated = DateTime.Now.ToUniversalTime();
                order.IsAlive = true;
                order.Title = "Заказ " + order.Number + " от " + order.Date.Value.ToString("dd.MM.yyyy HH.MM");
                order.Author = userInfo.AppUser.FullName;
                await _uow.Orders.AddAsync(order);
            }
            else
            {

                _uow.Orders.Update(order);
            }
            if (order.Status == "Новый" && (order.LogisticCompanyId > 0))
            {
                order.Status = "Предложен";
                _uow.Orders.Update(order);
            }
            
            try
            {
                var distance = await GetDistanceTotalAsync(orderDto);
                if (distance > 0)
                {
                    order.TotalDistance = (int)distance;
                    _uow.Orders.Update(order);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await _uow.SaveChangesAsync();
            //await _offersService.SetAutoCost(this, _contragentService, _logisticPriceService, order.Id);
            await SetAutoCost(currentOrderId: order.Id);
            return _mapper.Map<Order, OrderDto>(order!); ;
        }

        public OrderDto? Update(OrderDto itemDto)
        {
            if (itemDto == null) return null;
            var res = _uow.Orders.Update(_mapper.Map<OrderDto, Order>(itemDto));
            //_offersService.SetAutoCost(this, _contragentService, _logisticPriceService, itemDto.Id);
             SetAutoCost(currentOrderId: itemDto.Id).Wait();
            return _mapper.Map<Order, OrderDto>(res);


        }

        public async Task<ICollection<OrderDto>> GetPagedListForUserAsync(UserInfo userInfo, OrderFilterDto filterDto)
        {
            userInfo = await _identityService.GetUserInfo();
            var items = await _uow.Orders.GetPagedAsync(filterDto);
            return (_mapper.Map<List<Order>, List<OrderDto>>(items)).Where(o => AccessDocumentForView(o, userInfo)).ToList();
        }
        public async Task<ICollection<OrderDto>> GetPagedListDashboardForUserAsync(OrderFilterDto filterDto)
        {
            var userInfo = await _identityService.GetUserInfo();
            var items = await _uow.Orders.GetPagedDashboardAsync(filterDto);
            return (_mapper.Map<List<Order>, List<OrderDto>>(items)).Where(o => AccessDocumentForView(o, userInfo)).ToList();
        }

        public async Task<OrderDto?> MakeOfferToLogisticCompany(OrderDto orderDto, long logist)
        {
            orderDto.LogisticCompanyId = logist;
            orderDto.Status = "Предложен";
            var result = Update(orderDto);
            await _uow.SaveChangesAsync();
            return result;
        }

        public async Task<OrderDto?> CancelOfferToLogisticCompany(OrderDto orderDto)
        {
            orderDto.LogisticCompanyId = null;
            orderDto.LogisticCompany = null;
            orderDto.Status = "Новый";
            var result = Update(orderDto);
            await _uow.SaveChangesAsync();
            //await _offersService.SetAutoCost(this, _contragentService, _logisticPriceService, orderDto.Id);
            await SetAutoCost(currentOrderId: orderDto.Id);
            return result;
        }

        public async Task TakeOrderToWork(OrderDto orderDto, long logistId, LogisticOffer offer)
        {
            if (orderDto.Status == "Предложен" && orderDto.LogisticCompany?.Id == logistId)
            {
                orderDto.Status = "В работе";

                var deliveryContract = new DeliveryContractDto()
                {
                    DateDelivery = orderDto.DateDeparture,
                    LogisticCompanyId = logistId,
                    Order = orderDto,
                    OrderId = orderDto.Id,
                    Status = "В работе",
                    TotalCostDelivery = offer.Amount,
                    Date = DateTime.Now.ToUniversalTime(),


                };
                deliveryContract.Number = _uow.Numerators.GetNextStringNumber(ObjectTypes.DeliveryContract, logistId, deliveryContract.Date);
                Update(orderDto);
                await _deliveryContractService.AddAsync(deliveryContract);
                await _uow.SaveChangesAsync();
                _offersService.RemoveOfferDeliveryOrder(orderDto.Id);
            }
            else { throw new BadRequestException("Этот заказ вам нельзя взять в работу"); }
        }

        public async Task<OrderDto?> RefuseOrder(OrderDto orderDto, long logist)
        {
            OrderDto? result = null;
            if (orderDto.LogisticCompanyId == logist)
            {
                orderDto.LogisticCompany = null;
                orderDto.LogisticCompanyId = null;
                orderDto.Status = "Новый";
                result = Update(orderDto);
                await _uow.SaveChangesAsync();
                //await _offersService.SetAutoCost(this, _contragentService, _logisticPriceService, orderDto.Id);
                await SetAutoCost(currentOrderId: orderDto.Id);
            }
            else
            {
                throw new AccessDeniedException($"Заказ {orderDto.Number}");
            }
            return result;
        }

        public async Task<OrderDto?> SetStatusToOrder(UserInfo userInfo, OrderDto orderDto, string status)
        {
            OrderDto? result = null;
            var dbOrder = GetDocumentById(orderDto.Id);
            var allowedStatuses = AllowedEditStatuses(userInfo);
            if (allowedStatuses.Contains(status))
            {
                dbOrder.Status = status;
                result = Update(dbOrder);
                await _uow.SaveChangesAsync();
            }
            else throw new AccessDeniedException($"Статус заказа {status}");
            return result;
        }

        public static List<string> AllowedEditStatuses(UserInfo userInfo)
        {
            if (userInfo == null) return new List<string>();
            if (userInfo.Logist) return new List<string>() { "В работе", "Отклонен", "Отгружен" };
            if (userInfo.Customer) return new List<string>() { "Черновик", "Новый", "Предложен", "Претензия", "Завершен" };
            if (userInfo.Administrator) return new List<string>() { "Черновик", "В работе", "Отгружен", "Завершен", "Новый", "Претензия" };
            return new List<string>();
        }

        //public ICollection<LogisticOffer> GetBestOffersAsync(OrderDto order)
        //{
        //    var offers = _offersService.LogisticOffersList[order];
        //    return offers;
        //}
        //public async void SetAutoCost()
        //{
        //    var allLogists = (await _contragentService.GetPagedAsync(new ContragentFilterDto() { NotActive = true }))
        //        .Where(c => c.LogisticCompany).ToList();

        //    var allOrders = (await GetActiveOrderListAsync()).Where(o => o.Status == "Новый");

        //    foreach (var order in allOrders)
        //    {
        //        if (!_offersService.LogisticOffersList.ContainsKey(order))
        //        {
        //            _offersService.LogisticOffersList.TryAdd(order, new List<LogisticOffer>());
        //        }

        //        var offers = _offersService.LogisticOffersList[order];

        //        foreach (var logist in allLogists)
        //        {
        //            var finded = offers.FirstOrDefault(of => of.LogisticCompany.Id == logist.Id);
        //            var price = (await _logisticPriceService.GetBookByIdAsync(0));
        //            var cost = price?.CostStart ?? 0 + price?.CostPerTnKm ?? 0 * order.TotalDistance * (order.Invoices?.Sum(i => i.Weight) ?? 0);
        //            var newoffer = new LogisticOffer { LogisticCompany = logist, Amount = cost, isAuto = true };
        //            if (finded == null)
        //            {

        //                _offersService.LogisticOffersList[order].Add(newoffer);
        //                foreach (LogisticOffer obj in _offersService.LogisticOffersList[order])
        //                {
        //                    if (obj.LogisticCompany.Id == logist.Id && obj.isAuto)
        //                    {
        //                        obj.Amount = cost;
        //                        break;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                _offersService.LogisticOffersList[order].Add(newoffer);
        //            }
        //        }


        //    }
        //}

        public async Task SetAutoCost(long? currentLogistId = null, long? currentOrderId = null)
        {
            await _offersService.SetAutoCost(this, _contragentService, _logisticPriceService, currentLogistId, currentOrderId);

        }

        public async Task<decimal> GetDistanceTotalAsync(OrderDto order)
        {
           var invoices = GetInvoices(order);
           
           return (await _geoService.GetGeoData(invoices)).Distance;
        }

        public async Task<decimal> GetTimeTotalAsync(OrderDto order)
        {
            return 0;
        }


    }
}
