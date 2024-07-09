using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace userapi.Mappers
{
    public class AppUser : IdentityUser
    {
        public string? Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserStatus { get; set; }
    }
}