
using NotifyBotApi.Models.MailService;

namespace AuthApi.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(Message email);
    }
}
