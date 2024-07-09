using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace userapi.Dtos.Account
{
    public class RegisterBrandDto
    {
        [Required]
        public string? BrandName { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Hotline { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? UserStatus { get; set; } // Thêm thuộc tính Status
    }
}