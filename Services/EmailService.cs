using System.Net;
using System.Net.Mail;

namespace SistemaReservas.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string destino, string asunto, string contenidoHtml)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];

            using var message = new MailMessage();
            message.From = new MailAddress(senderEmail!, "Sistema de Reservas");
            message.To.Add(new MailAddress(destino));
            message.Subject = asunto;
            message.Body = contenidoHtml;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpServer, port);
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.EnableSsl = true;

            await client.SendMailAsync(message);
        }
    }
}