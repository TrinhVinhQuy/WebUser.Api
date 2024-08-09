using WebUser.Domain.Model;

namespace WebUser.Domain.Abstracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(CancellationToken cancellationToken, EmailRequest emailRequest);
        Task<string> GetTemplate(string templateName);
    }
}
