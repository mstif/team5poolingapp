
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationUsers;
namespace ApplicationUsers
{
    public class UserInfo
    {
        public UserInfo() { }
        public UserInfo(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            AppUser = user;
            Administrator = userManager.IsInRoleAsync(user, UserRoles.Administrator).Result;
            Customer = userManager.IsInRoleAsync(user, UserRoles.Customer).Result;
            Logist = userManager.IsInRoleAsync(user, UserRoles.Logist).Result;

        }
        public ApplicationUser AppUser { get; set; }
        public bool Administrator { get; set; }
        public bool Customer { get; set; }
        public bool Logist { get; set; }
  

        public SettingsUser? Settings => SettingsUser.GetFromJson(AppUser.SettingsUser);




    }
}
