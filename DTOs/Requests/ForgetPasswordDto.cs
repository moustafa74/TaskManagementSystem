using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Requests
{
    public class ForgetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
