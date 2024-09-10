using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BLL;
using System.Threading.Tasks;
using TaskManagementSystem.DTOs.Requests;
using TaskManagementSystem.DTOs.Responses;

namespace TaskManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserService _userService;

        public RegisterController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterationRequsetDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(registrationDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPatch]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            var result = await _userService.ConfirmEmailAsync(confirmEmailDto.UserId, confirmEmailDto.Token);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(new { message = "Email confirmed successfully." });
        }
    }
}
