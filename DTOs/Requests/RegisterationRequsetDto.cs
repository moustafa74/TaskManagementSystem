using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Requests;

public class RegisterationRequsetDto
{
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

}
