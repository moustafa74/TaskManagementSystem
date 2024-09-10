using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Configuration;

public class EmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService(IOptionsMonitor<EmailSettings> optionsMonitor)
    {
        var emailSettings = optionsMonitor.CurrentValue;

        _smtpClient = new SmtpClient(emailSettings.SmtpServer)
        {
            Port = emailSettings.Port,
            Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
            EnableSsl = true
        };
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        try
        {
            using (var mailMessage = new MailMessage("expertfloow@outlook.com", email))
            {
                mailMessage.Subject = "Confirm your email";
                mailMessage.Body = $"Please confirm your account by clicking on the following link: {confirmationLink}";
                mailMessage.IsBodyHtml = true;

                await _smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
    public async Task SendPasswordResetEmailAsync(string email, string resetPasswordLink)
    {
        try
        {
            using (var mailMessage = new MailMessage("expertfloow@outlook.com", email))
            {
                mailMessage.Subject = "Reset your password";
                mailMessage.Body = $"Please reset your password by clicking on the following link: {resetPasswordLink}";
                mailMessage.IsBodyHtml = true;

                await _smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}
