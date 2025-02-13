namespace Farmer_Project.Service
{
    public interface EmailService
    {
        Task<string> SendEmail();
    }
}
