using E_Commerce.Models.DTOs;
using E_Commerce.Models.Entities;
using E_Commerce.Models.ViewModels;
using E_Commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _accountService.ValidateUserAsync(loginDto);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                var token = await _accountService.GenerateJwtTokenAsync(user);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Convert RegisterDto to RegisterVM
                var registerVM = new RegisterVM
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                    ConfirmPassword = registerDto.ConfirmPassword,
                    UserName = registerDto.FullName,
                    PhoneNumber = registerDto.PhoneNumber ?? "",
                    Address = registerDto.Address ?? ""
                };

                var result = await _accountService.RegisterAsync(registerVM);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Registration failed", errors });
                }

                // Get the created user and generate token
                var user = await _accountService.GetUserByEmailAsync(registerDto.Email);
                if (user == null)
                    return StatusCode(500, new { message = "User created but could not be retrieved" });

                var token = await _accountService.GenerateJwtTokenAsync(user);
                return CreatedAtAction(nameof(GetProfile), new { }, token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutAsync();
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null)
                    return Unauthorized(new { message = "User not found" });

                var userProfile = await _accountService.GetUserProfileAsync(userId);
                if (userProfile == null)
                    return NotFound(new { message = "User profile not found" });

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null)
                    return Unauthorized(new { message = "User not found" });

                var result = await _accountService.UpdateUserProfileAsync(userId, userDto);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Profile update failed", errors });
                }

                var updatedProfile = await _accountService.GetUserProfileAsync(userId);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null)
                    return Unauthorized(new { message = "User not found" });

                var result = await _accountService.ChangePasswordAsync(userId, changePasswordDto);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Password change failed", errors });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var token = await _accountService.GeneratePasswordResetTokenAsync(forgotPasswordDto.Email);
                if (string.IsNullOrEmpty(token))
                    return NotFound(new { message = "User not found" });

                // In a real application, you would send this token via email
                // For now, we'll return it in the response (for testing purposes)
                return Ok(new { 
                    message = "Password reset token generated successfully",
                    token = token // Remove this in production
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _accountService.ResetPasswordAsync(resetPasswordDto);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Password reset failed", errors });
                }

                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("verify-token")]
        [Authorize]
        public ActionResult VerifyToken()
        {
            try
            {
                var userId = _accountService.GetCurrentUserId(User);
                if (userId == null)
                    return Unauthorized(new { message = "Invalid token" });

                return Ok(new { message = "Token is valid", userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("user-info")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            try
            {
                var user = await _accountService.GetCurrentUserAsync(User);
                if (user == null)
                    return Unauthorized(new { message = "User not found" });

                var userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FullName = user.UserName!,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    IsActive = user.IsActive,
                    CreatedDate = user.LockoutEnd?.DateTime
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
