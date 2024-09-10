using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementSystem.Configuration;
using TaskManagementSystem.DTOs.Requests;
using TaskManagementSystem.DTOs.Responses;
using System.Collections.Generic;
using System.Net;

namespace TaskManagementSystem.BLL
{
    public class UserService
    {
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptionsMonitor<JwtConfig> _optionsMonitor;
        private readonly EmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IGenericRepository<ApplicationUser> userRepository, UserManager<ApplicationUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, EmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _optionsMonitor = optionsMonitor;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponeDTO> RegisterUserAsync(RegisterationRequsetDto userDto)
        {
            var existingUser=new ApplicationUser();
            try
            {
                 existingUser = (await _userRepository.GetAllAsync(u => u.Email == userDto.Email)).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            
            if (existingUser != null)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = new List<string> { "Email already in use" }
                };
            }
            var newUser = new ApplicationUser
            {
                Email = userDto.Email,
                FullName = userDto.FullName,
                UserName = userDto.Email
            };

            var result = await _userManager.CreateAsync(newUser, userDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = result.Errors.Select(e => e.Description).ToList()
                };
            }
            var roleResult = await _userManager.AddToRoleAsync(newUser, "RegularUser");
            if (!roleResult.Succeeded)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = roleResult.Errors.Select(e => e.Description).ToList()
                };
            }
                await _userRepository.SaveAsync();

            SendEmailConfirmation(newUser);
            return new AuthResponeDTO
            {
                Success = true
            };
        }
        public async void SendEmailConfirmation(ApplicationUser user)
        {
            // Generate confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generate confirmation link
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var confirmationLink = $"{baseUrl}/Pages/ConfirmEmail.html?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

        }
        public async Task<AuthResponeDTO> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponeDTO { Success = false, Error = new List<string> { "User not found." } };
            }

            var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation", token);
            if (!isValidToken)
            {
                return new AuthResponeDTO { Success = false, Error = new List<string> { "Invalid token." } };
            }

            user.EmailConfirmed = true;
            await _userRepository.UpdateAsync(user);

            return new AuthResponeDTO { Success = true };
        }
        public async Task<AuthResponeDTO> LoginUserAsync(LoginRequestDto loginDto)
        {
            var user = (await _userRepository.GetAllAsync(u => u.Email == loginDto.UserName)).FirstOrDefault();
            if (user == null)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = new List<string> { "Invalid Email" }
                };
            }

            // Password verification using UserManager
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = new List<string> { "Invalid Password" }
                };
            }
            if (!user.EmailConfirmed)
            {
                SendEmailConfirmation(user);
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = new List<string> { "Email Not Confirmed,We Sent Email For Conformation Now ,Please Confirm your Email." }           
                };
            }
            // Generate the JWT token
            var token = await GenerateJwtToken(user);
            return new AuthResponeDTO
            {
                Success = true,
                User_ID=user.Id,
                Token = token
            };
        }


        public async Task<AuthResponeDTO> ForgetPasswordAsync(string email)
        {
            var user = (await _userRepository.GetAllAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = new List<string> { "Email not found." }
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var resetPasswordLink = $"{baseUrl}/Pages/ResetPassword.html?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

            await _emailService.SendPasswordResetEmailAsync(user.Email, resetPasswordLink);

            return new AuthResponeDTO
            {
                Success = true,
                User_ID=user.Id
            };
        }
        public async Task<AuthResponeDTO> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(resetPasswordDto.UserId);
            if (user == null)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = new List<string> { "User not found." }
                };
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!resetPasswordResult.Succeeded)
            {
                return new AuthResponeDTO
                {
                    Success = false,
                    Error = resetPasswordResult.Errors.Select(e => e.Description).ToList()
                };
            }

            return new AuthResponeDTO
            {
                Success = true
            };
        }


        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_optionsMonitor.CurrentValue.Secret);
            var Expire = _optionsMonitor.CurrentValue.ExpirationInMinutes;

            var userRoles = await _userManager.GetRolesAsync(user); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }.Concat(userRoles.Select(role => new Claim(ClaimTypes.Role, role)))),
                Expires = DateTime.UtcNow.AddMinutes(Expire),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
