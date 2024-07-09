using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using userapi.Dtos.Account;
using userapi.Interfaces;
using userapi.Mappers;

namespace userapi.Controllers
{
    [Route("userapi/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        // private readonly AppBrandManager _brandManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signinManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            // _brandManager = brandManager;
            _tokenService = tokenService;
            _signinManager = signinManager;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username!");

            if (user.UserStatus != "Available")
            {
                return Ok("Account is not active.");
            }

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found or Password is incorrect");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    UserStatus = user.UserStatus,
                    Token = _tokenService.CreateToken(user),
                    Roles = roles
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new AppUser
                {
                    Fullname = registerDto.Fullname,
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    UserStatus = registerDto.UserStatus // Gán trạng thái
                    // Other properties
                };

                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                Token = _tokenService.CreateToken(user)
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, new { Message = "An error occurred while assigning the role." });
                    }
                }
                else
                {
                    foreach (var error in createdUser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                // Log the exception (ensure logger is configured and injected)
                _logger.LogError(e, "An error occurred while registering the user.");
                return StatusCode(500, new { Message = "An error occurred while registering the user." });
            }
        }

        // [HttpPost("registerBrand")]
        // public async Task<IActionResult> RegisterBrand([FromBody] RegisterBrandDto registerBrandDto)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     try
        //     {
        //         var brand = new AppBrand
        //         {
        //             BrandName = registerBrandDto.BrandName,
        //             UserName = registerBrandDto.Username,
        //             Email = registerBrandDto.Email,
        //             Hotline = registerBrandDto.Hotline,
        //             UserStatus = registerBrandDto.UserStatus
        //         };

        //         var createdUser = await _brandManager.CreateAsync(brand, registerBrandDto.Password);

        //         if (createdUser.Succeeded)
        //         {
        //             var roleResult = await _brandManager.AddToRoleAsync(brand, "Brand");
        //             if (roleResult.Succeeded)
        //             {
        //                 return Ok(
        //                     new NewBrandDto
        //                     {
        //                         BrandName = brand.BrandName,
        //                         Email = brand.Email,
        //                         Token = _tokenService.CreateToken(brand)
        //                     }
        //                 );
        //             }
        //             else
        //             {
        //                 return StatusCode(500, new { Message = "An error occurred while assigning the role." });
        //             }
        //         }
        //         else
        //         {
        //             foreach (var error in createdUser.Errors)
        //             {
        //                 ModelState.AddModelError(string.Empty, error.Description);
        //             }
        //             return BadRequest(ModelState);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         // Log the exception (ensure logger is configured and injected)
        //         _logger.LogError(e, "An error occurred while registering the user.");
        //         return StatusCode(500, new { Message = "An error occurred while registering the user." });
        //     }
        // }

        [HttpPut("update/{username}")]
        public async Task<IActionResult> UpdateAsync(string username, [FromBody] UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Message = "Username is required" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                user.Fullname = updateUserDto.Fullname;
                user.Email = updateUserDto.Email;
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.UserStatus = updateUserDto.UserStatus; // Cập nhật trạng thái

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(
                        new NewUserDto
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Token = _tokenService.CreateToken(user)
                        }
                    );
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut("updateStatus/{username}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateUserStatus(string username, [FromBody] string newStatus)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Message = "Username is required" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                user.UserStatus = newStatus; // Cập nhật trạng thái

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User status updated successfully" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }


        [HttpDelete("delete/{username}")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> Delete(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User deleted successfully" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
    // public class AppBrandManager : UserManager<AppBrand>
    // {
    //     public AppBrandManager(IUserStore<AppBrand> store, IOptions<IdentityOptions> optionsAccessor,
    //         IPasswordHasher<AppBrand> passwordHasher, IEnumerable<IUserValidator<AppBrand>> userValidators,
    //         IEnumerable<IPasswordValidator<AppBrand>> passwordValidators, ILookupNormalizer keyNormalizer,
    //         IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppBrand>> logger)
    //         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    //     {
    //     }
    // }
}
