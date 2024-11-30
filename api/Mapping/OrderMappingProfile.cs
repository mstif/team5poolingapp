using api.Models;
using api.Models.CargoType;
using api.Models.Contragents;
using api.Models.DeliveryContract;
using api.Models.LogisticPrice;
using api.Models.Orders;
using ApplicationCore.Books;
using ApplicationCore.Documents;
using ApplicationCore.Entities.Books;
using ApplicationUsers;
using AutoMapper;
using Services.Contracts;

namespace api.Mapping
{
    public class OrderMappingProfile : Profile
    {

        public OrderMappingProfile()
        {
            CreateMap<Contragent, ContragentDto>();

            CreateMap<CargoType, CargoTypeDto>();

            CreateMap<CargoTypeDto, EntityModel>()
                .ForMember(op => op.Title, (options) => options.MapFrom(o => o.Name));

            CreateMap<ContragentDto, EntityModel>()
                .ForMember(op => op.Title, (options) => options.MapFrom(o => o.Name));

            //CreateMap<EntityModel, ContragentDto>()
            //    .ForMember(op => op.Name, (options) => options.MapFrom(o => o.Title))
            //    .ForMember(op => op.Comment, (options) => options.Ignore())
            //    .ForMember(op => op.Address, (options) => options.Ignore())
            //    .ForMember(op => op.ClientCompany, (options) => options.Ignore())
            //    .ForMember(op => op.Country, (options) => options.Ignore())
            //    .ForMember(op => op.DeliveryPoint, (options) => options.Ignore())
            //    .ForMember(op => op.INN, (options) => options.Ignore())
            //    .ForMember(op => op.IsAlive, (options) => options.Ignore())
            //    .ForMember(op => op.Latitude, (options) => options.Ignore())
            //    .ForMember(op => op.Longitude, (options) => options.Ignore())
            //    .ForMember(op => op.LogisticCompany, (options) => options.Ignore());


            //CreateMap<EntityModel, CargoTypeDto>()
            //.ForMember(op => op.Name, (options) => options.MapFrom(o => o.Title))
            //.ForMember(op => op.Comment, (options) => options.Ignore())
            //.ForMember(op => op.IsAlive, (options) => options.Ignore());


            //CreateMap<EntityModel, InvoiceDto>()
            //.ForMember(op => op.Title, (options) => options.MapFrom(o => o.Title))
            //.ForMember(op => op.Comment, (options) => options.Ignore())
            //.ForMember(op => op.IsAlive, (options) => options.Ignore())
            //.ForMember(op => op.Number, (options) => options.Ignore())
            //.ForMember(op => op.BoxAmount, (options) => options.Ignore())
            //.ForMember(op => op.Date, (options) => options.Ignore())
            //.ForMember(op => op.PallettAmount, (options) => options.Ignore())
            //.ForMember(op => op.DateDeliveryFrom, (options) => options.Ignore())
            //.ForMember(op => op.DateDeliveryUpTo, (options) => options.Ignore())
            //.ForMember(op => op.Order, (options) => options.Ignore())
            //.ForMember(op => op.OrderId, (options) => options.Ignore())
            //.ForMember(op => op.DeliveryPoint, (options) => options.Ignore())
            //.ForMember(op => op.DeliveryPointId, (options) => options.Ignore())
            //.ForMember(op => op.PallettAmount, (options) => options.Ignore())
            //.ForMember(op => op.StatusDoc, (options) => options.Ignore())
            //.ForMember(op => op.TotalCost, (options) => options.Ignore())
            //.ForMember(op => op.Weight, (options) => options.Ignore());


            CreateMap<ApplicationUser, EntityModelGuid>()
                .ForMember(op => op.Title, (options) => options.MapFrom(o => o.FullName));

            CreateMap<InvoiceDto, EntityModel>()
                .ForMember(op => op.Title, (options) => options.MapFrom(o => o.Title + " от " + o.Date));

            CreateMap<Order, OrderDto>();
            //.ForMember(op => op.Seller, (options) => options.MapFrom(o => o.Seller))
            //.ForMember(op => op.LogisticCompany, (options) => options.MapFrom(o => o.LogisticCompany))
            //.ForMember(op => op.CargoType, (options) => options.MapFrom(o => o.CargoType))
            //.ForMember(op => op.Invoices, (options) => options.MapFrom(o => o.Invoices));

            CreateMap<Invoice, InvoiceDto>()
                .ForMember(op => op.Order, (options) => options.Ignore());
            //.ForMember(op => op.DeliveryPoint, (options) => options.MapFrom(o => o.DeliveryPoint));

            CreateMap<ContragentDto, Contragent>();


            CreateMap<OrderDto, Order>();
            //.ForMember(op => op.Seller, (options) => options.MapFrom(o => o.Seller))
            //.ForMember(op => op.LogisticCompany, (options) => options.MapFrom(o => o.LogisticCompany))
            //.ForMember(op => op.CargoType, (options) => options.MapFrom(o => o.CargoType))
            //.ForMember(op => op.Invoices, (options) => options.MapFrom(o => o.Invoices));

            CreateMap<InvoiceDto, Invoice>()
                .ForMember(op => op.Order, (options) => options.Ignore())
                .ForMember(op => op.DeliveryPoint, (options) => options.MapFrom(o => o.DeliveryPoint));

            CreateMap<OrderFilterModel, OrderFilterDto>();

            CreateMap<OrderFilterDto, OrderFilterModel>();
            CreateMap<InvoiceDto, InvoiceModel>()
                .ForMember(op => op.OrderModel, (options) => options.Ignore());
                //.ForMember(op => op.DeliveryPoint, (options) => options.Ignore());
           
            CreateMap<InvoiceDto, EntityInvoiceModel>();

            CreateMap<InvoiceModel, InvoiceDto>()
            .ForMember(op => op.Order, (options) => options.Ignore())
            .ForMember(op => op.DeliveryPoint, (options) => options.Ignore());

            CreateMap<OrderDto, OrderModel>()
            .ForMember(op => op.TotalWeight, (options) => options.Ignore())
            .ForMember(op => op.AvailableActions, (options) => options.Ignore())
            .ForMember(op => op.LogisticOffers, (options) => options.Ignore())
            .ForMember(op => op.DeliveryContract, (options) => options.Ignore())
            //.ForMember(op => op.Date, (options) => options.MapFrom(o => o.Date.Value.ToShortTimeString()))
            //.ForMember(op => op.DateDeparture, (options) => options.MapFrom(o => o.DateDeparture.Value.ToShortDateString()));
            //.ForMember(op => op.Seller, (options) => options.MapFrom(o => o.Seller))
            //.ForMember(op => op.LogisticCompany, (options) => options.MapFrom(o => o.LogisticCompany))
            //.ForMember(op => op.Author, (options) => options.MapFrom(o => o.Author))
            .ForMember(op => op.Invoices, (options) => options.MapFrom(o => o.Invoices));


            CreateMap<OrderModel, OrderDto>()
                .ForMember(op => op.Author, (options) => options.Ignore())
                .ForMember(op => op.CargoType, (options) => options.Ignore())
                .ForMember(op => op.Seller, (options) => options.Ignore())
                .ForMember(op => op.LogisticCompany, (options) => options.Ignore())
                .ForMember(op => op.Invoices, (options) => options.Ignore())
                .ForMember(op => op.CargoTypeId, (options) => options.MapFrom(o => o.CargoType.Id))
                .ForMember(op => op.SellerId, (options) => options.MapFrom(o => o.Seller.Id))
                .ForMember(op => op.LogisticCompanyId, (options) => options.MapFrom(o => o.LogisticCompany.Id));

            CreateMap<ContragentDto, ContragentModel>()
                .ForMember(op => op.AllCountries, (options) => options.Ignore());
            CreateMap<ContragentFilterModel, ContragentFilterDto>();
            CreateMap<ContragentModel, ContragentDto>();

            CreateMap<CargoTypeDto, CargoTypeModel>();
            CreateMap<CargoTypeDto, CargoType>();
            CreateMap<CargoTypeModel, CargoTypeDto>();

            CreateMap<LogisticPriceDto, LogisticPriceModel>();
            CreateMap<LogisticPriceDto, LogisticPrice>();
            CreateMap<LogisticPriceModel, LogisticPriceDto>();
            CreateMap<LogisticPrice, LogisticPriceDto>();

            CreateMap<DeliveryContractDto, DeliveryContractCreated>();
            CreateMap<DeliveryContractDto, DeliveryContractModel>();
            CreateMap<DeliveryContractDto, DeliveryContract>();
            CreateMap<DeliveryContractCreated, DeliveryContractDto>();
            CreateMap<DeliveryContractModel, DeliveryContractDto>();
            CreateMap<DeliveryContract, DeliveryContractDto>();
            CreateMap<DeliveryContractFilterModel, DeliveryContractFilterDto>();
            CreateMap<DeliveryContractFilterDto, DeliveryContractFilterModel>();
        }
    }
}
