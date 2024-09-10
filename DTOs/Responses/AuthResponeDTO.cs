namespace TaskManagementSystem.DTOs.Responses;

public class AuthResponeDTO
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string User_ID { get; set; }
    public List<string> Error { get; set; }
}
