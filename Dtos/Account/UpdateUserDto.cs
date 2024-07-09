using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace userapi.Dtos.Account
{
    public class UpdateUserDto
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserStatus { get; set; } // Thêm thuộc tính Status
    }
}