using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Requests;

public class LoginRequestDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
