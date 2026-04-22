using E_Commerce.Models.DTOs;
using E_Commerce.Models.ViewModels.AccountVM;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;


        [HttpPost("login")]
        public async Task<ActionResult<TokenDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _accountService.ValidateUserAsync(loginDTO);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                var token = await _accountService.GenerateJwtTokenAsync(user);

                return Ok(token);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpPost("register")]
        public async Task<ActionResult<TokenDTO>> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var existingUser = await _accountService.GetUserByEmailAsync(registerDTO.Email.Trim());
                if (existingUser != null)
                    return Conflict(new { message = "Email is already registered" });

                var registerVM = new RegisterVM
                {
                    UserName = registerDTO.Email.Trim(),
                    Password = registerDTO.Password,
                    PhoneNumber = registerDTO.PhoneNumber,
                    Address = registerDTO.Address
                };

                var result = await _accountService.RegisterAsync(registerVM);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);

                    return BadRequest(new { message = "Registration failed", errors });
                }

                var user = await _accountService.GetUserByEmailAsync(registerDTO.Email.Trim());
                if (user == null) return StatusCode(500, new { message = "User created but could not be retrieved" });

                var token = await _accountService.GenerateJwtTokenAsync(user);

                return Ok(token);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutAsync();
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserDTO>> GetProfile()
        {
            try
            {
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null) return Unauthorized(new { message = "User not found" });

                var userProfile = await _accountService.GetUserProfileAsync(userId);
                if (userProfile == null) return NotFound(new { message = "User profile not found" });

                return Ok(userProfile);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserDTO>> UpdateProfile([FromBody] UserDTO userDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null) return Unauthorized(new { message = "User not found" });

                var result = await _accountService.UpdateUserProfileAsync(userId, userDTO);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);

                    return BadRequest(new { message = "Profile update failed", errors });
                }

                var updatedProfile = await _accountService.GetUserProfileAsync(userId);

                return Ok(updatedProfile);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null) return Unauthorized(new { message = "User not found" });

                var result = await _accountService.ChangePasswordAsync(userId, changePasswordDTO);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);

                    return BadRequest(new { message = "Password change failed", errors });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpGet("verify-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult VerifyToken()
        {
            try
            {
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null) return Unauthorized(new { message = "Invalid token" });

                return Ok(new { message = "Token is valid", userId });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpGet("user-info")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserDTO>> GetUserInfo()
        {
            try
            {
                var user = await _accountService.GetCurrentUserAsync(User);
                if (user == null) return Unauthorized(new { message = "User not found" });

                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Address = user.Address,
                    LastLoginDate = user.LockoutEnd?.DateTime
                };

                return Ok(userDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}