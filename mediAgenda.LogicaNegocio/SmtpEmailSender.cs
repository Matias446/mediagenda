using mediAgenda.ILogicaNegocio;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace mediAgenda.LogicaNegocio;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        var host = _configuration["Email:SmtpHost"] ?? throw new InvalidOperationException("Falta configurar Email:SmtpHost");
        var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var username = _configuration["Email:Username"] ?? throw new InvalidOperationException("Falta configurar Email:Username");
        var password = _configuration["Email:Password"] ?? throw new InvalidOperationException("Falta configurar Email:Password");
        var from = _configuration["Email:From"] ?? throw new InvalidOperationException("Falta configurar Email:From");

        var mensaje = new MimeMessage();
        mensaje.From.Add(MailboxAddress.Parse(from));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("plain") { Text = cuerpo };

        using var cliente = new SmtpClient();
        await cliente.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await cliente.AuthenticateAsync(username, password);
        await cliente.SendAsync(mensaje);
        await cliente.DisconnectAsync(true);
    }
}
