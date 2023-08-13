
using AuthApi.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models.MailService;

namespace AuthApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailConfig.DisplayName, emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            /* gửi với nội dung có thể ở định dạng HTML */
            //var bodyBuilder = new BodyBuilder();
            //bodyBuilder.HtmlBody = message.Content;
            //emailMessage.Body = bodyBuilder.ToMessageBody();
            // hoặc
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

            /* gửi với nội dung văn bản thuần túy */
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        public async Task<bool> SendEmail(Message email)
        {
            bool isSent = true;
            // tạo email
            var emailMessage = CreateEmailMessage(email);
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(emailConfig.SmtpServer, emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfig.UserName, emailConfig.Password);
                    await client.SendAsync(emailMessage);
                }
                catch
                {
                    isSent = false;
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
            return isSent;
        }
    }
}

