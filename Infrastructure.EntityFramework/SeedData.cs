using ApplicationCore.Books;
using ApplicationCore.Documents;
using ApplicationCore.Entities.Books;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts;


namespace Otus.Teaching.PromoCodeFactory.DataAccess.Data
{
    public class SeedData
    {
        private readonly IUnitOfWork _db;

        public SeedData(IUnitOfWork db)
        {
            _db = db;

        }
        public void Seed()
        {
            ////////// точки доставки
            Contragent DeliveryPoint = new Contragent()
            {
                Address = "Москва, Красная площадь,1",
                ClientCompany = false,
                DeliveryPoint = true,
                LogisticCompany = false,
                Country = Countries.Russia,
                INN = "7789045773",
                Name = "Точка доставки 1",
                IsAlive = true,
                Latitude= "55.755246",
                Longitude= "37.617779"

            };
            Contragent DeliveryPoint1 = new Contragent()
            {
                Address = "Москва, ул. Лужники, 24, стр. 1",
                ClientCompany = false,
                DeliveryPoint = true,
                LogisticCompany = false,
                Country = Countries.Russia,
                INN = "7789058223",
                Name = "Точка доставки 2",
                IsAlive = true,
                Latitude= "55.715677",
                Longitude = "37.552166"

            };
            Contragent DeliveryPoint2 = new Contragent()
            {
                Address = "1-й пер. Тружеников, 6, Москва",
                ClientCompany = false,
                DeliveryPoint = true,
                LogisticCompany = false,
                Country = Countries.Russia,
                INN = "7789345523",
                Name = "Точка доставки 3",
                IsAlive = true,
                Latitude= "55.739042",
                Longitude= "37.574795"

            };
            ////////////////////////////////////////////////////продавцы
            Contragent Seller = new Contragent()
            {
                Address = "Москва, Краснопресненская наб., 2",
                ClientCompany = true,
                DeliveryPoint = false,
                LogisticCompany = false,
                Country = Countries.Russia,
                INN = "77899555421",
                Name = "Продавец 1",
                IsAlive = true,
                Latitude = "55.755186",
                Longitude = "37.573384"
            };

            Contragent Seller1 = new Contragent()
            {
                Address = "Часовая ул., 23, корп. 1, Москва",
                ClientCompany = true,
                DeliveryPoint = false,
                LogisticCompany = false,
                Country = Countries.Russia,
                INN = "77890099455421",
                Name = "Продавец 2",
                IsAlive = true
            };
            ////////////////////////////////логисты

            Contragent Logist = new Contragent()
            {
                Address = "Москва, Лаврушинский пер., 10, стр. 4",
                ClientCompany = false,
                DeliveryPoint = false,
                LogisticCompany = true,
                Country = Countries.Russia,
                INN = "77890464521",
                Name = "Логист 1",
                IsAlive = true,
                Latitude= "55.809980",
                Longitude="37.521830"
            };

            Contragent Logist1 = new Contragent()
            {
                Address = "Планетная ул., 45, стр. 2, Москва",
                ClientCompany = false,
                DeliveryPoint = false,
                LogisticCompany = true,
                Country = Countries.Russia,
                INN = "77890464521",
                Name = "Логист 2",
                IsAlive = true,
                Latitude= "55.805361",
                Longitude = "37.539293"
            };



            var deliveryPoint = _db.Contragents.Add(DeliveryPoint);
            var deliveryPoint1 = _db.Contragents.Add(DeliveryPoint1);
            var deliveryPoint2 = _db.Contragents.Add(DeliveryPoint2);
            var logist = _db.Contragents.Add(Logist);
            var logist1 = _db.Contragents.Add(Logist1);
            var seller = _db.Contragents.Add(Seller);
            var seller1 = _db.Contragents.Add(Seller1);


            var CargoType = new CargoType() { IsAlive = true, Name = "Промышленные товары" };
            var cargoType = _db.CargoTypes.Add(CargoType);
            _db.SaveChanges();



            var price1 = new LogisticPrice()
            {
                Name = "p1",
                CostPerTnKm = 310,
                CostStart = 4588,
                LogisticCompanyId = Logist.Id
            };


            var price2 = new LogisticPrice()
            {
                Name = "p2",
                CostPerTnKm = 360,
                CostStart = 3000,
                LogisticCompanyId = logist1.Id
            };

            _db.LogisticPrices.Add(price1);
            _db.LogisticPrices.Add(price2);


            Order order = new Order()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                DateCreated = DateTime.Now.ToUniversalTime(),
                Comment = "Первый заказ",
                DateDeparture = DateTime.Now.AddDays(3).ToUniversalTime(),
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Order, seller.Id, DateTime.Now.ToUniversalTime()),
                Seller = seller,
                Status = "Новый",
                Title = "Заказ на доставку 1",
                CargoType = cargoType,
                TotalDistance= 4374
            };
            var orderRes = _db.Orders.Add(order);
            _db.SaveChanges();

            Invoice invoice1 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = orderRes.Id,
                PallettAmount = 10,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 1",
                TotalCost = 25000,
                Weight = 2600


            };
            Invoice invoice2 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = order.Id,
                PallettAmount = 10,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint2,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 2",
                TotalCost = 29000,
                Weight = 2300


            };
            _db.Invoices.AddRange(new List<Invoice> { invoice1, invoice2 });
            _db.SaveChanges();

            ////////////////////////////////////////////////////
            order = new Order()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                DateCreated = DateTime.Now.ToUniversalTime(),
                Comment = "Второй заказ",
                DateDeparture = DateTime.Now.AddDays(3).ToUniversalTime(),
                
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Order, seller.Id, DateTime.Now.ToUniversalTime()),
                Seller = seller1,
                Status = "Новый",
                Title = "Заказ на доставку 2",
                CargoType = cargoType,
                TotalDistance = 11448,
            };
            orderRes = _db.Orders.Add(order);
            _db.SaveChanges();

            Invoice invoice3 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = orderRes.Id,
                PallettAmount = 10,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 3",
                TotalCost = 45000,
                Weight = 2000


            };
            Invoice invoice4 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = order.Id,
                PallettAmount = 10,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint1,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 4",
                TotalCost = 25000,
                Weight = 2600


            };
            Invoice invoice5 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = order.Id,
                PallettAmount = 10,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint2,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 5",
                TotalCost = 22000,
                Weight = 3600


            };

            _db.Invoices.AddRange(new List<Invoice> { invoice3, invoice4, invoice5 });
            _db.SaveChanges();
            ////////////////////////////////////////
            order = new Order()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                DateCreated = DateTime.Now.ToUniversalTime(),
                Comment = "Третий заказ",
                DateDeparture = DateTime.Now.AddDays(3).ToUniversalTime(),
                
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Order, seller.Id, DateTime.Now.ToUniversalTime()),
                Seller = seller,
                Status = "Новый",
                Title = "Заказ на доставку 3",
                CargoType = cargoType,
               // TotalDistance= 11536 

            };
            orderRes = _db.Orders.Add(order);
            _db.SaveChanges();

            Invoice invoice6 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = orderRes.Id,
                PallettAmount = 15,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint2,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 6",
                TotalCost = 30000,
                Weight = 2100


            };
            Invoice invoice7 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = order.Id,
                PallettAmount = 4,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 7",
                TotalCost = 24000,
                Weight = 2800


            };
            Invoice invoice8 = new Invoice()
            {
                IsAlive = true,
                Date = DateTime.Now.ToUniversalTime(),
                OrderId = order.Id,
                PallettAmount = 15,
                StatusDoc = InvoiceStatus.New,
                DeliveryPoint = deliveryPoint1,
                Number = _db.Numerators.GetNextStringNumber(ObjectTypes.Invoice, deliveryPoint.Id, DateTime.Now.ToUniversalTime()),
                Title = "Накладная 8",
                TotalCost = 22000,
                Weight = 3600


            };

            _db.Invoices.AddRange(new List<Invoice> { invoice6, invoice7, invoice8 });
            _db.SaveChanges();


        }
       
        

    
    }

}
