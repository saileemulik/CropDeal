namespace CropDeal.Repository;

public class EmailServiceRepository : IEmailServiceRepository
{
    private readonly IConfiguration _config;

    public EmailServiceRepository(IConfiguration config)
    {
        _config = config;
    }
public async Task SendEmailAsync(string toEmail, string subject, string body)
{
    try
    {
        var smtpClient = new SmtpClient(_config["Smtp:Host"])
        {
            Port = int.Parse(_config["Smtp:Port"]),
            Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["Smtp:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };
        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
        Console.WriteLine("✅ Email sent to " + toEmail);
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Failed to send email: " + ex.Message);
        throw;
    }
}

}
