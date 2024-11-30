
//using ApplicationCore.Interfaces;
//using ApplicationUsers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore.Storage;
//using Microsoft.Extensions.Caching.Memory;
//using Services.Abstractions;
//using Services.Implementation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace App.Services.Services
//{
//    public class CommonServices : ICommonServices
//    {
//        IUnitOfWork Database { get; set; }
//        UserManager<ApplicationUser> UserManagerService { get; set; }
//        IHttpContextAccessor HttpContextAccessorService { get; set; }
//        IMemoryCache MemoryCache { get; set; }
//        public CommonServices(IUnitOfWork uow,
//             UserManager<ApplicationUser> userManager,
//            IHttpContextAccessor httpContextAccessor,
//            IMemoryCache cache)
//        {
//            Database = uow;

//            CargoTypeService = new CargoTypeService(Database);
//            ContragentService = new ContragentService(Database);

//            OrderService = new OrderService(Database, userManager, httpContextAccessor);
//            InvoiceService = new InvoiceService(Database);
//            NumeratorService = new NumeratorService(Database);
//            ContractService = new ContractService(Database, userManager, httpContextAccessor);
//            UserManagerService = userManager;
//            HttpContextAccessorService = httpContextAccessor;

//            MemoryCache = cache;
//        }
//        public StockService StockService { get; set; }
//        public CargoTypeService CargoTypeService { get; set; }
//        public ContragentService ContragentService { get; set; }
//        public TemperatureService TemperatureService { get; set; }
//        public OrderService OrderService { get; set; }
//        public InvoiceService InvoiceService { get; set; }
//        public NumeratorService NumeratorService { get; set; }









//        public UserInfo? GetUserInfo(ApplicationUser? user)
//        {

//            UserInfo? userInfo;
//            if (user != null)
//            {
//                if (MemoryCache.TryGetValue("usersInfoKey", out Dictionary<string, UserInfo>? usersKeys))
//                {

//                    if (usersKeys != null)
//                    {
//                        usersKeys.TryGetValue(user.Id, out userInfo);
//                        if (userInfo != null)
//                        {

//                            return userInfo;
//                        }
//                    }
//                }

//                userInfo = new UserInfo(user, UserManagerService);
//                if (userInfo != null)
//                {
//                    return userInfo;
//                }
//            }
//            return null;

//        }


//        public async Task<UserInfo?> GetUserInfo(ClaimsPrincipal user)
//        {

//            ApplicationUser? appUser = await UserManagerService.GetUserAsync(user);
//            return GetUserInfo(appUser);


//        }

//    }

//}
