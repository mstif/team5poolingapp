using Services.Abstractions;
using Services.Contracts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using System.Text.Json;
using ApplicationUsers;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;

namespace Services.Implementation
{
    public class IdentityService(IUnitOfWork uow,
       UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor
       ):IIdentityService
    {
        IUnitOfWork Database { get; set; } = uow;
        UserManager<ApplicationUser> _userManager { get; set; } = userManager;
        IHttpContextAccessor _httpContextAccessor { get; set; } = httpContextAccessor;

        public async Task<UserInfo> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return new UserInfo(user, _userManager);
        }



    }
}
