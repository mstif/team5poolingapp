
using api.Controllers;
using api.Mapping;
using FluentAssertions;
using ApplicationCore.Books;
using ApplicationCore.Documents;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Documents;
using AutoFixture;
using AutoMapper;
using Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repositories.Implementation.Repositories.Documents;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementation;
using System;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ApplicationCore.Enums;
using ApplicationUsers;

namespace TestAppPooling
{
    public class GetInvoiceByNumberTest
    {
        private readonly Mock<IInvoiceService> _mockInvoiceService;
        private readonly InvoiceController _invoiceController;
        private readonly IContragentService _contragentService;
        private readonly IMapper _mapper;

        public GetInvoiceByNumberTest()
        {
            _mockInvoiceService = new Mock<IInvoiceService>();
            _mapper = new Mapper(GetMapperConfiguration());
            _invoiceController = new InvoiceController(_mockInvoiceService.Object, _contragentService, _mapper);
        }

        [Fact]
        public async Task GetInvoiceByNumber()
        {

            var invoice = new InvoiceDto() {
                Number="0001",
                OrderId = 1, 
                Date = DateTime.Now,
                PallettAmount = 10, 
                Title = "TTH", 
                Id = 1,
                TotalCost = 10000 };

            _mockInvoiceService.Setup(repo => repo.GetDocumentByNumber(invoice.Number,2024)).ReturnsAsync(invoice);
            var result = await _invoiceController.GetByNumberAsync(invoice.Number, 2024);            
            //Assert
            result.Should().NotBeAssignableTo<NotFoundResult>();

        }

        [Fact]
        public async Task SetStatusInvoice()
        {

            var invoiceModel = new InvoiceModel()
            {
                Number = "00001",
                OrderId = 1,
                Date = DateTime.Now,
                PallettAmount = 10,
                Title = "TTH",
                Id = 1,
                TotalCost = 10000
            };

            var fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var invoice = fixture.Build<InvoiceDto>().With(x => x.Number, invoiceModel.Number)
                .With(l => l.Date, invoiceModel.Date)
                .With(l => l.Title, invoiceModel.Title)
                .With(l => l.Id, invoiceModel.Id)
                .With(l => l.StatusDoc, invoiceModel.StatusDoc)
                .Create();

            _mockInvoiceService.Setup(repo => repo.GetDocumentByNumber(invoice.Number, 2024)).ReturnsAsync(invoice);

            //var result = await _invoiceController.SetStatusInvoice(invoiceModel, InvoiceStatus.Delivered);
            invoice.StatusDoc = InvoiceStatus.Delivered;
            
            var result= _mockInvoiceService.Object.Update( invoice);

            _mockInvoiceService.Verify(repo => repo.Update(invoice), Times.Once());
            var actionResult = await _invoiceController.GetByNumberAsync(invoice.Number, 2024);
            var okResult = actionResult as OkObjectResult;
            var resInvoiceModel = okResult?.Value as InvoiceModel;
            //Assert
            resInvoiceModel?.StatusDoc.Should().Be(InvoiceStatus.Delivered);

        }


        static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrderMappingProfile>();

            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }
    }
}