using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationUsers;
using Services.Contracts.Helpers;
namespace Services.Abstractions
{
    public interface IIdentityService
    {
        Task<UserInfo> GetUserInfo();
    }
}
