using JobPlatformBackend.Business.src.Managers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using MailKit.Net.Smtp;
 using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;



namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class EmailService : IEmailService
	{
		private readonly EmailOption _settings;
		public EmailService(IOptions<EmailOption> settings)
		{
			_settings = settings.Value;
		}
		public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
		{
			try
			{
				var email=new MimeMessage();
				email.From.Add(new MailboxAddress("Doroob", _settings.FromEmail));
				email.To.Add(new MailboxAddress("",toEmail));

				email.Subject = subject;
				email.Body=new TextPart("html")
				{
					Text = htmlMessage
				};
				using var smtp=new SmtpClient();
				await smtp.ConnectAsync(_settings.SmtpServer,_settings.Port, SecureSocketOptions.SslOnConnect);
				await smtp.AuthenticateAsync(_settings.FromEmail, _settings.Password);
				await smtp.SendAsync(email);
				await smtp.DisconnectAsync(true);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
