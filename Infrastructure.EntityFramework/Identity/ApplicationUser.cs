using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ApplicationUsers
{
    public class ApplicationUser:IdentityUser
    {
        public string? FullName { get; set; }
        public bool? Approved { get; set; } = true;

        public string? SettingsUser { get; set; }
    }
}
