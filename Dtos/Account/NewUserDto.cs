using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace userapi.Dtos.Account
{
    public class NewUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public string UserStatus { get; set; }
        public IList<string> Roles { get; set; }
    }
}