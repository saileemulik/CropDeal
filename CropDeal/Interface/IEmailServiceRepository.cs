namespace CropDeal.Interface;

    public interface IEmailServiceRepository
    {
    Task SendEmailAsync(string toEmail, string subject, string body);
    }
